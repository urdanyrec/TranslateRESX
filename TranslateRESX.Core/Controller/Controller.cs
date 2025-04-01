using TranslateRESX.DB;
using TranslateRESX.Domain.Models;
using TranslateRESX.Domain.Enums;
using TranslateRESX.Domain.Helpers;
using TranslateRESX.Core.Translators;
using System.Collections;
using System.Resources.Extensions;
using TranslateRESX.Core.Helpers;

namespace TranslateRESX.Core.Controller
{
    public class Controller : IController
    {
        public CancellationTokenSource CancellationTokenSource { get; }

        public event EventHandler Finished;
        public event EventHandler Started;

        private string _pathToBackup;

        public Controller(string pathToBackup)
        {
            CancellationTokenSource = new CancellationTokenSource();
            _pathToBackup = pathToBackup;
        }


        public Task Start(IParametersModel parameters, IContainer dbContainer, IState state, CancellationToken cancellationToken)
        {
            if (parameters == null)
                throw new NullReferenceException();

            return Task.Factory.StartNew(() => 
            {
                state.State = StateType.NotRunning;
                try
                {
                    if (cancellationToken.IsCancellationRequested)
                        cancellationToken.ThrowIfCancellationRequested();

                    state.State = StateType.Processing;

                    string filename = Path.GetFileNameWithoutExtension(parameters.SourceFilename);
                    string directory = Path.GetDirectoryName(parameters.SourceFilename);
                    string extension = Path.GetExtension(parameters.SourceFilename);

                    string sourceLanguage = LanguageHelper.LanguageDictionary[parameters.SourceLanguage];
                    string targetLanguage = LanguageHelper.LanguageDictionary[parameters.TargetLanguage];

                    // Создание бекапа файлов
                    string backupPath = Path.Combine(_pathToBackup, DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
                    Directory.CreateDirectory(backupPath);
                    File.Copy(filename, Path.Combine(backupPath, Path.GetFileName(filename)));


                    var langMatch = System.Text.RegularExpressions.Regex.Match(Path.GetFileNameWithoutExtension(filename), @"\.([a-z]{2,3})$");
                    string targetFilename;
                    if (langMatch.Success)
                    {
                        // Замена существующего языкового кода
                        targetFilename = Path.Combine(directory, filename.Substring(0, langMatch.Index) + $".{targetLanguage}" + extension);
                    }
                    else
                    {
                        // Добавление нового языкового кода
                        targetFilename = Path.Combine(directory, filename + $".{targetLanguage}" + extension);
                    }

                    // Выбор сервиса для перевода
                    ITranslator translator = null;
                    switch (parameters.Service)
                    {
                        case LanguageService.Yandex:
                            translator = new YandexTranslator(parameters.ApiKey, targetLanguage);
                            break;
                    }

                    using (DeserializingResourceReader reader = new DeserializingResourceReader(filename))
                    using (PreserializedResourceWriter writer = new PreserializedResourceWriter(targetFilename))
                    {
                        foreach (DictionaryEntry entry in reader)
                        {
                            if (entry.Value is string text)
                            {
                                Task<TranslateTaskResult> result = translator.TranslateTextAsync(text, sourceLanguage);
                                result.Wait();
                                if (result.Result.StatusCode == 200)
                                {
                                    string translatedText = result.Result.TranslatedPhrase;
                                    writer.AddResource(entry.Key.ToString(), translatedText);
                                }
                            }
                            else
                            {
                                writer.AddResource(entry.Key.ToString(), entry.Value);
                            }
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    state.State = StateType.Canceled;
                    throw;
                }
                catch (Exception ex)
                {
                    state.State = StateType.Error;
                    throw ex;
                }

            }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        protected virtual void OnStarted()
        {
            Started?.Invoke(this, EventArgs.Empty);
        }

        protected virtual void OnFinished()
        {
            Finished?.Invoke(this, EventArgs.Empty);
        }
    }
}
