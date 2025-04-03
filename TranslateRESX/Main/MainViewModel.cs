using Caliburn.Micro;
using System.Dynamic;
using System.Runtime.Remoting.Messaging;
using System.Threading;
using System.Windows;
using System;
using TranslateRESX.Domain.Enums;
using TranslateRESX.Domain.Models;
using TranslateRESX.TranslateParameters;
using TranslateRESX.TranslateState;
using TranslateRESX.Core.Controller;
using TranslateRESX.DB;
using System.Data;
using TranslateRESX.Core.Events;

namespace TranslateRESX.Main
{
    public class MainViewModel : PropertyChangedBase, IMainView
    {
        private readonly IWindowManager _windowManager;

        public MainViewModel(IWindowManager windowManager)
        {
            _windowManager = windowManager;
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

                Controller.CancellationTokenSource = new CancellationTokenSource();
                Controller.StateChanged += ControllerStateChanged;
                await Controller.Start(TranslateParametersView, Container);     
            }
            catch (OperationCanceledException ex)
            {
                var a = ex;
            }
            catch (Exception ex)
            {
                var a = ex;
            }
            finally
            {
                Controller.StateChanged -= ControllerStateChanged;
                TranslationStarted = false;
            }
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
