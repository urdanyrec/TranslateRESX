using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using TranslateRESX.ApiHistory;
using TranslateRESX.Core.Controller;
using TranslateRESX.DB;
using TranslateRESX.Dialog;
using TranslateRESX.Domain.Models;
using TranslateRESX.Helpers;
using TranslateRESX.Main;
using TranslateRESX.TranslateParameters;
using TranslateRESX.TranslateState;

namespace TranslateRESX
{
    public class Bootstrapper : BootstrapperBase
    {
        private SimpleContainer _container;

        public Bootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            _container = new SimpleContainer();
            _container.Singleton<IWindowManager, CustomWindowManager>();
            _container.Singleton<IEventAggregator, EventAggregator>();
            _container.Singleton<ITranslateParametersView, TranslateParametersViewModel>();
            _container.Singleton<ITranslateStateView, TranslateStateViewModel>();
            _container.Singleton<IMainView, MainViewModel>();
            _container.Singleton<IApiHistoryView, ApiHistoryViewModel>();
            _container.PerRequest<IDialogView, DialogViewModel>();

            var config = new VisualConfig();
            _container.RegisterInstance(typeof(IVisualConfig), "Config", config);

            var container = new Container();
            _container.RegisterInstance(typeof(IContainer), "Db", container);       

            var controller = new Controller(container.DatabaseDirectory);
            _container.RegisterInstance(typeof(IController), "Controller", controller);
        }
        protected override object GetInstance(Type service, string key)
        {
            var instance = _container.GetInstance(service, key);
            if (instance != null)
                return instance;

            throw new InvalidOperationException("Could not locate any instances.");
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _container.GetAllInstances(service);
        }

        protected override void BuildUp(object instance)
        {
            _container.BuildUp(instance);
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            Process proc = Process.GetCurrentProcess();
            int count = Process.GetProcesses().Count(p => p.ProcessName == proc.ProcessName);
            if (count > 1)
            {
                MessageBox.Show("Программа уже запущена");
                Environment.Exit(-1);
            }

            var configVisual = IoC.Get<IVisualConfig>();
            try
            {
                configVisual.Read();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            DisplayRootViewFor<IMainView>();
        }

        protected override void OnExit(object sender, EventArgs e)
        {
            var configVisual = IoC.Get<IVisualConfig>();
            configVisual.Write();

            var container = IoC.Get<IContainer>();
            container.Dispose();
        }

        protected override void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (!(e.Exception is TaskCanceledException))
            {
                MessageBox.Show($"{e.Exception?.Message}\n{e.Exception?.InnerException?.Message}",
                                "Ошибка",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
            e.Handled = true;
        }
    }
}
