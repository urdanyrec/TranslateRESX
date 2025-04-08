using AutoMapper;
using Caliburn.Micro;
using System;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Windows;
using TranslateRESX.AddLanguage;
using TranslateRESX.Db.Entity;
using TranslateRESX.DB;
using TranslateRESX.ViewModel;

namespace TranslateRESX.LanguageEditing
{
    public class LanguageEditingViewModel : PropertyChangedBase, ILanguageEditingView
    {
        private readonly IMapper _mapper;
        private readonly IWindowManager _windowManager;
        private IContainer _container => IoC.Get<IContainer>();

        public LanguageEditingViewModel(IWindowManager windowManager)
        {
            _windowManager = windowManager;
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Language, LanguageViewModel>();
            });
            _mapper = mapperConfiguration.CreateMapper();
        }

        private ObservableCollection<LanguageViewModel> _languages = new ObservableCollection<LanguageViewModel>();
        public ObservableCollection<LanguageViewModel> Languages
        {
            get => _languages;
            set
            {
                _languages = value;
                NotifyOfPropertyChange();
            }
        }

        private LanguageViewModel _selectedLanguage;
        public LanguageViewModel SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                _selectedLanguage = value;
                NotifyOfPropertyChange();
            }
        }

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

        public void LoadedCommand()
        {
            try
            {
                UpdateLanguages();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void ClosedCommand()
        {
            foreach (var language in Languages)
            {
                var existedEntity = _container.Languages.Find(x => x.Id == language.Id).FirstOrDefault();
                var existedModel = _mapper.Map<Language, LanguageViewModel>(existedEntity);
                if (existedModel != null && !existedModel.Equals(language) && !existedModel.IsDefault)
                {
                    existedEntity.LocalizationSuffix = language.LocalizationSuffix;
                    existedEntity.LanguageCode = language.LanguageCode;
                    _container.Complete();
                }
            }
            var events = IoC.Get<IEventAggregator>();
            events.PublishOnCurrentThread("LanguagesUpdated");
        }

        public void AddCommand()
        {
            dynamic viewSettings = new ExpandoObject();
            IAddLanguageView addLanguageViewModel = (AddLanguageViewModel)IoC.Get<IAddLanguageView>();
            var dialogResult = _windowManager.ShowDialog(addLanguageViewModel, settings: viewSettings) == true;
            if (dialogResult != true)
                return;

            if (addLanguageViewModel != null && !string.IsNullOrEmpty(addLanguageViewModel.LanguageName))
            {
                _container.Languages.Add(new Language 
                { 
                    LanguageName = addLanguageViewModel.LanguageName,
                    LanguageCode = "",
                    LocalizationSuffix = ""
                });
                _container.Complete();

                UpdateLanguages();
                SelectedLanguage = Languages.FirstOrDefault(x => x.LanguageName == addLanguageViewModel.LanguageName);
            }
        }

        public void DeleteCommand()
        {
            if (SelectedLanguage == null)
                return;

            var message = $"Вы уверены, что хотите удалить {SelectedLanguage.LanguageName} язык?";
            var dialogResult = MessageBox.Show(message, "Удаление языка", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (dialogResult == MessageBoxResult.Yes)
            {
                var findedEntity = _container.Languages.Get(SelectedLanguage.Id);
                _container.Languages.Remove(findedEntity);
                _container.Complete();
                UpdateLanguages();
            }
        }

        private void UpdateLanguages()
        {
            Languages.Clear();
            var languages = _container.Languages.GetAll();
            foreach (var col in languages)
                Languages.Add(_mapper.Map<Language, LanguageViewModel>(col));
            SelectedLanguage = Languages.FirstOrDefault();
        }
    }
}
