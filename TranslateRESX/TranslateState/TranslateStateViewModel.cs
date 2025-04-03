using Caliburn.Micro;
using System.Windows;
using TranslateRESX.Domain.Enums;
using TranslateRESX.Core.Events;
using TranslateRESX.Domain.Models;
using TranslateRESX.TranslateParameters;

namespace TranslateRESX.TranslateState
{
    public class TranslateStateViewModel : PropertyChangedBase, ITranslateStateView, IHandle<StateChangedEventArgs>
    {
        private readonly IWindowManager _windowManager;

        private TranslateStateView _view;

        public TranslateStateViewModel(IWindowManager windowManager, IEventAggregator events)
        {
            _windowManager = windowManager;
            events.Subscribe(this);
        }

        private StateType _state;
        public StateType State
        {
            get => _state;
            set
            {
                _state = value;
                NotifyOfPropertyChange();
            }
        }

        public double _progress;
        public double Progress
        {
            get => _progress;
            set
            {
                _progress = value;
                NotifyOfPropertyChange();
            }
        }

        private int _currentIndex;
        public int CurrentIndex
        {
            get => _currentIndex;
            set
            {
                _currentIndex = value;
                NotifyOfPropertyChange();
            }
        }

        private int _allCount;
        public int AllCount
        {
            get => _allCount;
            set
            {
                _allCount = value;
                NotifyOfPropertyChange();
            }
        }

        private readonly object _syncLock = new object();
        private string _log;
        public string Log
        {
            get => _log;
            set
            {
                _log = value;
                NotifyOfPropertyChange();
            }
        }

        public void Handle(StateChangedEventArgs args)
        {
            try
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    State = args.CurrentState.State;
                    Progress = args.CurrentState.Progress;
                    CurrentIndex = args.CurrentState.CurrentIndex;
                    AllCount = args.CurrentState.AllCount;
                    lock (_syncLock)
                    {
                        Log = args.CurrentState.Log;
                        _view.LogTextBox.ScrollToEnd();
                    }
                }, System.Windows.Threading.DispatcherPriority.Background);
            }
            catch { }
        }

        public void LoadedCommand(TranslateStateView view)
        {
            _view = view;
        }
    }
}
