using Caliburn.Micro;
using System.Dynamic;
using System.Threading;
using System.Windows;
using System;
using TranslateRESX.Domain.Enums;
using TranslateRESX.Domain.Models;
using TranslateRESX.TranslateParameters;
using TranslateRESX.TranslateState;
using TranslateRESX.Core.Controller;
using TranslateRESX.DB;
using TranslateRESX.Core.Events;
using TranslateRESX.Dialog;
namespace TranslateRESX.Main
{
    public class MainViewModel : PropertyChangedBase, IMainView
    {
        private readonly IWindowManager _windowManager;

        private CancellationTokenSource _cancellationTokenSource;

        public MainViewModel(IWindowManager windowManager)
        {
            _windowManager = windowManager;
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public ITranslateParametersView TranslateParametersView => IoC.Get<ITranslateParametersView>();

        public ITranslateStateView TranslateStateView => IoC.Get<ITranslateStateView>();

        public IController Controller => IoC.Get<IController>();

        public IContainer Container => IoC.Get<IContainer>();

        private bool _translationStarted;
        public bool TranslationStarted
        {
            get => _translationStarted;
            set
            {
                _translationStarted = value;
                NotifyOfPropertyChange();
            }
        }


        public async void StartCommand()
        {
            try
            {
                TranslationStarted = true;

                _cancellationTokenSource = new CancellationTokenSource();
                Controller.StateChanged += ControllerStateChanged;

                var result = await Controller.Start(TranslateParametersView, Container, _cancellationTokenSource.Token);
                if (result.Success)
                {
                    dynamic settings = new ExpandoObject();
                    var dialog = IoC.Get<IDialogView>();
                    dialog.Title = "Информация";
                    dialog.Message = $"Перевод выполнен успешно. \n";
                    dialog.BoldMessage = result.CompleteCount.ToString();
                    if (result.TargetFileExists)
                        dialog.Message += $"К файлу ресурсов добавлено {result.CompleteCount} строк.";
                    else
                        dialog.Message += $"Переведено {result.CompleteCount} строк.";

                    _windowManager.ShowDialog(dialog, settings: settings);
                }
                else
                {
                    dynamic settings = new ExpandoObject();
                    var dialog = IoC.Get<IDialogView>();
                    dialog.Title = "Ошибка";
                    dialog.Message = $"Произошла ошибка во время перевода. \n";
                    dialog.Error = true;
                    if (result.TargetFileRecovered)
                        dialog.Message += $"Выходной файл ресурсов был восстановлен по умолчанию.\n";
                    switch (result.Error)
                    {
                        case OperationCanceledException canceledException:
                            dialog.Message += $"Перевод был отменен.";
                            break;

                        case InvalidOperationException invalidException:
                            dialog.Message += invalidException.Message;
                            break;

                        case Exception exception:
                            dialog.Message += exception.Message;
                            break;
                    }

                    _windowManager.ShowDialog(dialog, settings: settings);
                }
            }
            finally
            {
                Controller.StateChanged -= ControllerStateChanged;
                TranslationStarted = false;
            }
        }

        public void StopCommand()
        {
            _cancellationTokenSource?.Cancel();
        }

        public void LoadedCommand()
        {
            try
            {
                var visualConfig = IoC.Get<IVisualConfig>();
                TranslateParametersView.ApiKey = visualConfig.ApiKey;
                TranslateParametersView.Service = visualConfig.Service;
                TranslateParametersView.SourceLanguage = visualConfig.SourceLanguage;
                TranslateParametersView.TargetLanguage = visualConfig.TargetLanguage;
            }
            catch { }
        }

        public void ClosedCommand()
        {
            try
            {
                var visualConfig = IoC.Get<IVisualConfig>();
                visualConfig.ApiKey = TranslateParametersView.ApiKey;
                visualConfig.Service = TranslateParametersView.Service;
                visualConfig.SourceLanguage = TranslateParametersView.SourceLanguage;
                visualConfig.TargetLanguage = TranslateParametersView.TargetLanguage;
            }
            finally
            {
                try
                {
                    Application.Current.Shutdown();
                }
                catch (Exception) { }
            }
        }

        private void ControllerStateChanged(object sender, StateChangedEventArgs e)
        {
            if (e.CurrentState.State != StateType.Completed)
            {
                var events = IoC.Get<IEventAggregator>();
                events.PublishOnUIThread(e);
            }
        }
    }
}
