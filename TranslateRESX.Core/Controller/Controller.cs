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
using System.Xml.Linq;

namespace TranslateRESX.Core.Controller
{
    public class Controller : IController
    {
        public event EventHandler<StateChangedEventArgs> StateChanged;

        private string _pathToBackup;

        public Controller(string pathToBackup)
        {
            _pathToBackup = pathToBackup;
        }

        public Task<ControllerTaskResult> Start(IParametersModel parameters, IContainer dbContainer, CancellationToken cancellationToken)
        {
            if (parameters == null)
                throw new NullReferenceException();

            return Task<ControllerTaskResult>.Factory.StartNew(() => 
            {
                ControllerTaskResult controllerTaskResult = new ControllerTaskResult();
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

                // Восстановление бекапа при ошибке
                Action<bool> recoverBackup = (fileExists) =>
                {
                    if (!fileExists)
                        return;

                    File.Delete(parameters.TargetFilename);
                    File.Copy(Path.Combine(backupPath, Path.GetFileName(parameters.TargetFilename)), parameters.TargetFilename);
                };

                try
                {
                    // Словарь для хранения существующих переводов (если файл есть)
                    var existingResources = new Dictionary<string, object>();
                    var otherResources = new List<ResXDataNode>();
                    if (targetFileExists)
                    {
                        try
                        {
                            using (ResXResourceReader reader = new ResXResourceReader(parameters.TargetFilename))
                            {
                                reader.UseResXDataNodes = true;
                                foreach (DictionaryEntry entry in reader)
                                {
                                    var node = (ResXDataNode)entry.Value;
                                    if (node.GetValue((ITypeResolutionService)null) is string text)
                                    {
                                        existingResources.Add(entry.Key.ToString(), text);
                                    }
                                }
                            }
                        }
                        catch
                        {
                            throw new InvalidOperationException("Не удалось прочитать существующий выходной файл ресурсов.");
                        }
                    }

                    // Ключи для перевода
                    var resourcesToTranslate = new List<ResXDataNode>();
                    using (ResXResourceReader reader = new ResXResourceReader(parameters.SourceFilename))
                    {
                        reader.UseResXDataNodes = true;
                        foreach (DictionaryEntry entry in reader)
                        {
                            var node = (ResXDataNode)entry.Value;
                            try
                            {
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
                            catch
                            {
                                otherResources.Add(node);
                            }
                        }
                    }

                    if (resourcesToTranslate.Count == 0)
                        throw new InvalidOperationException("Отсутствуют уникальные ключи в файле ресурсов");

                    controllerTaskResult.AllCount = resourcesToTranslate.Count;
                    controllerTaskResult.TargetFileExists = targetFileExists;

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
                        case LanguageService.Google:
                            translator = new GoogleTranslator(parameters.ApiKey, targetLanguage);
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
                            // Проверка отмены
                            if (cancellationToken.IsCancellationRequested)
                                cancellationToken.ThrowIfCancellationRequested();

                            string text = resourcesToTranslate[i].GetValue((ITypeResolutionService)null) as string;
                            string translatedText = "";
                            Task<TranslateTaskResult> result = translator.TranslateTextAsync(text, sourceLanguage);
                            bool success = result.Wait(30000);
                            if (success && result.Result.StatusCode == 200)
                            {
                                translatedText = result.Result.TranslatedText;
                                var newNode = new ResXDataNode(resourcesToTranslate[i].Name, translatedText);
                                writer.AddResource(newNode);
                            }

                            // Обновление лога
                            state.CurrentIndex = i + 1;
                            state.Progress = 100.0 * (double)state.CurrentIndex / state.AllCount;
                            state.Log += $"{DateTime.Now}. " +
                                         $"Время ожидания: {result.Result.WaitTime} мс. " +
                                         $"Статус: {result.Result.StatusCode}." +
                                         $" Перевод: {sourceLanguage}-{targetLanguage}. " +
                                         $"Результат: {result.Result.TranslatedText}" + "\n";
                            OnStateChanged(state);

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

                            controllerTaskResult.CompleteCount = i + 1;
                        }
                    }

                    controllerTaskResult.Success = true;
                }
                catch (OperationCanceledException ex)
                {
                    state.State = StateType.Canceled;
                    OnStateChanged(state);
                    recoverBackup(targetFileExists);
                    controllerTaskResult.TargetFileRecovered = true;
                    controllerTaskResult.Success = false;
                    controllerTaskResult.Error = ex;
                }
                catch(InvalidOperationException ex)
                {
                    state.State = StateType.Error;
                    OnStateChanged(state);
                    controllerTaskResult.Success = false;
                    controllerTaskResult.Error = ex;
                }
                catch (Exception ex)
                {
                    state.State = StateType.Error;
                    OnStateChanged(state);
                    recoverBackup(targetFileExists);
                    controllerTaskResult.TargetFileRecovered = true;
                    controllerTaskResult.Success = false;
                    controllerTaskResult.Error = ex;
                }

                return controllerTaskResult;

            }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        protected virtual void OnStateChanged(IState state)
        {
            var eventArgs = new StateChangedEventArgs(state);
            StateChanged?.Invoke(this, eventArgs);
        }
    }
}
