using TranslateRESX.DB;
using TranslateRESX.Domain.Models;
using TranslateRESX.Domain.Enums;
using TranslateRESX.Core.Translators;
using System.Collections;
using TranslateRESX.Core.Helpers;
using TranslateRESX.DB.Entity;
using System.Threading;
using System;
using System.Threading.Tasks;
using System.IO;
using System.Collections.Generic;
using System.Resources;
using System.ComponentModel.Design;
using System.Diagnostics;
using TranslateRESX.Core.Events;

namespace TranslateRESX.Core.Controller
{
    public class Controller : IController
    {
        public CancellationTokenSource CancellationTokenSource { get; set; }

        public event EventHandler<StateChangedEventArgs> StateChanged;

        private string _pathToBackup;

        public Controller(string pathToBackup)
        {
            CancellationTokenSource = new CancellationTokenSource();
            _pathToBackup = pathToBackup;
        }

        public Task Start(IParametersModel parameters, IContainer dbContainer)
        {
            if (parameters == null)
                throw new NullReferenceException();

            return Task.Factory.StartNew(() => 
            {
                var state = new StateModel();
                state.State = StateType.NotRunning;
                OnStateChanged(state);

                // Создание бекапа файлов
                string backupPath = Path.Combine(_pathToBackup, DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
                Directory.CreateDirectory(backupPath);
                File.Copy(parameters.SourceFilename, Path.Combine(backupPath, Path.GetFileName(parameters.SourceFilename)));
                bool targetFileExists = File.Exists(parameters.TargetFilename);
                if (targetFileExists)
                    File.Copy(parameters.TargetFilename, Path.Combine(backupPath, Path.GetFileName(parameters.TargetFilename)));

                // Словарь для хранения существующих переводов (если файл есть)
                var existingResources = new Dictionary<string, object>();
                if (targetFileExists)
                {
                    try
                    {
                        using (ResXResourceReader reader = new ResXResourceReader(parameters.TargetFilename))
                        {
                            reader.UseResXDataNodes = true;
                            foreach (DictionaryEntry entry in reader)
                            {
                                if (entry.Value is string value)
                                {
                                    existingResources[entry.Key.ToString()] = value;
                                }
                            }
                        }
                    }
                    catch
                    {
                        File.Delete(parameters.TargetFilename);
                        targetFileExists = false;
                    }
                }

                // Ключи для перевода
                var resourcesToTranslate = new List<ResXDataNode>();
                var otherResources = new List<ResXDataNode>();
                using (ResXResourceReader reader = new ResXResourceReader(parameters.SourceFilename))
                {
                    reader.UseResXDataNodes = true;
                    foreach (DictionaryEntry entry in reader)
                    {
                        var node = (ResXDataNode)entry.Value;
                        if (node.GetValue((ITypeResolutionService)null) is string text)
                        {
                            if (!targetFileExists || !existingResources.ContainsKey(node.Name))
                            {
                                resourcesToTranslate.Add(node);
                            }
                        }
                        else if (!existingResources.ContainsKey(node.Name))
                        {
                            otherResources.Add(node);
                        }
                    }
                }

                try
                {
                    if (CancellationTokenSource.Token.IsCancellationRequested)
                        CancellationTokenSource.Token.ThrowIfCancellationRequested();

                    if (resourcesToTranslate.Count == 0)
                        throw new Exception("Отсутствуют уникальный ключи в файле ресурсов");

                    state.State = StateType.Processing;
                    state.Log = "";
                    state.AllCount = resourcesToTranslate.Count;
                    OnStateChanged(state);

                    string sourceLanguage = ResourceExtentions.LanguageDictionary[parameters.SourceLanguage].Item2;
                    string targetLanguage = ResourceExtentions.LanguageDictionary[parameters.TargetLanguage].Item2;

                    // Выбор сервиса для перевода
                    ITranslator translator = null;
                    switch (parameters.Service)
                    {
                        case LanguageService.Yandex:
                            translator = new YandexTranslator(parameters.ApiKey, targetLanguage);
                            break;
                        default:
                            translator = new Emulator(parameters.ApiKey, targetLanguage);
                            break;
                    }

                    using (ResXResourceWriter writer = new ResXResourceWriter(parameters.TargetFilename))
                    {
                        // Записываем существующие переводы и другие ресурсы
                        foreach (var existing in existingResources)
                        {
                            writer.AddResource(existing.Key, existing.Value);
                        }
                        foreach (var resource in otherResources)
                        {
                            writer.AddResource(resource);
                        }


                        // Добавляем новые переводы
                        for (int i = 0; i < resourcesToTranslate.Count; i++)
                        {
                            if (CancellationTokenSource.Token.IsCancellationRequested)
                                CancellationTokenSource.Token.ThrowIfCancellationRequested();

                            string text = resourcesToTranslate[i].GetValue((ITypeResolutionService)null) as string;
                            string translatedText = "";
                            Task<TranslateTaskResult> result = translator.TranslateTextAsync(text, sourceLanguage);
                            result.Wait();
                            if (result.Result.StatusCode == 200)
                            {
                                translatedText = result.Result.TranslatedPhrase;
                                var newNode = new ResXDataNode(resourcesToTranslate[i].Name, translatedText);
                                writer.AddResource(newNode);
                            }

                            // Обновление лога
                            state.Log += $"{DateTime.Now}. Время ожидания: {result.Result.WaitTime} мс. Статус: {result.Result.StatusCode}. Перевод: {sourceLanguage}-{targetLanguage}. Результат: {result.Result.TranslatedPhrase}" + "\n";
                            state.CurrentIndex = i;
                            state.Progress = 100.0 * (double)state.CurrentIndex / state.AllCount;
                            OnStateChanged(state);
                            Debug.WriteLine(state.Progress);
                            // Сохранение в БД
                            var dataEntity = new Data
                            {
                                Service = parameters.Service.GetDescription(),
                                DateTime = DateTime.Now,
                                DictinaryKey = resourcesToTranslate[i].Name,
                                ApiKey = parameters.ApiKey,
                                StatusCode = result.Result.StatusCode,
                                WaitTime = (int)result.Result.WaitTime,
                                RequestString = result.Result.Request,
                                AnswerString = result.Result.Answer,
                                SourceLanguage = sourceLanguage,
                                TargetLanguage = targetLanguage,
                                SourcePhrase = text,
                                TranslatedPhrase = translatedText
                            };
                            dbContainer.Results.Add(dataEntity);
                            dbContainer.Complete();
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    state.State = StateType.Canceled;
                    OnStateChanged(state);
                    throw;
                }
                catch (Exception ex)
                {
                    state.State = StateType.Error;
                    OnStateChanged(state);
                    throw ex;
                }
            }, CancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        protected virtual void OnStateChanged(IState state)
        {
            var eventArgs = new StateChangedEventArgs(state);
            StateChanged?.Invoke(this, eventArgs);
        }
    }
}
