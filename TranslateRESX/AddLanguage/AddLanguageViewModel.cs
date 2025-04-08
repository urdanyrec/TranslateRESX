using AutoMapper;
using Caliburn.Micro;
using TranslateRESX.Db.Entity;
using TranslateRESX.Domain.Models;

namespace TranslateRESX.AddLanguage
{
    public class AddLanguageViewModel : PropertyChangedBase, IAddLanguageView
    {
        private readonly IWindowManager _windowManager;
        private AddLanguageView _view;

        public AddLanguageViewModel(IWindowManager windowManager)
        {
            _windowManager = windowManager;
        }

        private string _languageName;
        public string LanguageName
        {
            get => _languageName;
            set
            {
                _languageName = value;
                NotifyOfPropertyChange();
            }
        }

        public void LoadedCommand(AddLanguageView view)
        {
            _view = view;
        }

        public void AddCommand()
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
