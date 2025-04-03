using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using TranslateRESX.Converters;
using TranslateRESX.DB;
using TranslateRESX.Domain.Models;

namespace TranslateRESX.ApiHistory
{
    public class ApiHistoryViewModel : PropertyChangedBase, IApiHistoryView
    {
        private readonly IWindowManager _windowManager;
        private ApiHistoryView _view;

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                NotifyOfPropertyChange();
            }
        }

        private BindableCollection<ApiKeyModel> _items = new BindableCollection<ApiKeyModel>();
        public BindableCollection<ApiKeyModel> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                NotifyOfPropertyChange();
            }
        }

        public ApiKeyModel _selectedApiKey;
        public ApiKeyModel SelectedApiKey
        {
            get => _selectedApiKey;
            set
            {
                _selectedApiKey = value;
                NotifyOfPropertyChange();
            }
        }

        public IContainer Container => IoC.Get<IContainer>();

        public ApiHistoryViewModel(IWindowManager windowManager)
        {
            _windowManager = windowManager;
        }

        public async void LoadedCommand(ApiHistoryView view)
        {
            _view = view;
            try
            {
                IsLoading = true;
                await Task.Factory.StartNew(() =>
                {
                    var data = Container.Results.GetUniqueApiKeys();
                    var apiModels = DataToApiModelConverter.Convert(data);
                    Application.Current.Dispatcher?.BeginInvoke(new System.Action(() =>
                    {
                        Items.Clear();
                        Items = apiModels;
                        SelectedApiKey = Items.FirstOrDefault();
                    }));
                }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
                IsLoading = false;
            }
            catch (Exception) { }
        }

        public void OkCommand()
        {
            _view.DialogResult = true;
            _view.Close();
        }

        public void CancelCommand()
        {
            _view.DialogResult = false;
            _view.Close();
        }
    }
}
