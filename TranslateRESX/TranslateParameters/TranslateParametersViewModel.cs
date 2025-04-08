using AutoMapper;
using Caliburn.Micro;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TranslateRESX.ApiHistory;
using TranslateRESX.Core.Helpers;
using TranslateRESX.Db.Entity;
using TranslateRESX.DB;
using TranslateRESX.Domain.Enums;
using TranslateRESX.Domain.Models;

namespace TranslateRESX.TranslateParameters
{
    public class TranslateParametersViewModel : PropertyChangedBase, ITranslateParametersView, IHandle<string>
    {
        private readonly IMapper _mapper;
        private readonly IWindowManager _windowManager;

        private IContainer _container => IoC.Get<IContainer>();

        public TranslateParametersViewModel(IWindowManager windowManager, IEventAggregator events)
        {
            _windowManager = windowManager;
            events.Subscribe(this);
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Language, LanguageModel>();
            });
            _mapper = mapperConfiguration.CreateMapper();
            UpdateLanguages();
        }

        private LanguageService _service;
        public LanguageService Service
        {
            get => _service;
            set
            {
                _service = value;
                NotifyOfPropertyChange();
            }
        }

        private string _apiKey;
        public string ApiKey
        {
            get => _apiKey;
            set
            {
                _apiKey = value;
                NotifyOfPropertyChange();
            }
        }

        private string _sourceFilename;
        public string SourceFilename
        {
            get => _sourceFilename;
            set
            {
                _sourceFilename = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(nameof(SourceFilenameWithExtention));
            }
        }

        public string SourceFilenameWithExtention
        {
            get 
            {
                string filename = "";
                try
                {
                    filename = Path.GetFileName(SourceFilename);
                }
                catch { }

                return filename;
            }
        }

        private string _targetDirectory;
        public string TargetDirectory
        {
            get => _targetDirectory;
            set
            {
                _targetDirectory = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(nameof(TargetFilename));
            }
        }

        private string _targetFilenameWithExtention;
        public string TargetFilenameWithExtention
        {
            get => _targetFilenameWithExtention;
            set
            {
                _targetFilenameWithExtention = value;
                NotifyOfPropertyChange();
                NotifyOfPropertyChange(nameof(TargetFilename));
            }
        }

        public string TargetFilename
        {
            get
            {
                var filename = "";
                try
                {
                    filename = Path.Combine(TargetDirectory, TargetFilenameWithExtention);
                }
                catch { }

                return filename;
            }
        }

        private ObservableCollection<LanguageModel> _languages = new ObservableCollection<LanguageModel>();
        public ObservableCollection<LanguageModel> Languages
        {
            get => _languages;
            set
            {
                _languages = value;
                NotifyOfPropertyChange();
            }
        }

        private string _sourceLanguageName;
        private LanguageModel _sourceLanguage;
        public LanguageModel SourceLanguage
        {
            get => _sourceLanguage;
            set
            {
                _sourceLanguage = value;
                NotifyOfPropertyChange();

                if (_sourceLanguage != null)
                    _sourceLanguageName = _sourceLanguage.LanguageName;
            }
        }

        private string _targetLanguageName;
        private LanguageModel _targetLanguage;
        public LanguageModel TargetLanguage
        {
            get => _targetLanguage;
            set
            {
                if (value == SourceLanguage)
                    return;

                _targetLanguage = value;
                NotifyOfPropertyChange();

                if (_targetLanguage != null)
                    _targetLanguageName = _targetLanguage.LanguageName;

                if (string.IsNullOrEmpty(TargetFilenameWithExtention))
                    return;

                var targetFilename = ResourceExtentions.GetFilenameWithExtention(TargetFilenameWithExtention, _targetLanguage.LocalizationSuffix);
                TargetFilenameWithExtention = Path.GetFileName(targetFilename);
            }
        }

        private bool _rewriteAllKeys;
        public bool RewriteAllKeys 
        {
            get => _rewriteAllKeys;
            set
            {
                _rewriteAllKeys = value;
                NotifyOfPropertyChange();
            }
        }

        public void LoadSourceFileCommand()
        {
            var openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Filter = $"Файл ресурсов (*.resx)|*.resx;";
            if (openFileDialog.ShowDialog() != true)
                return;

            var filename = openFileDialog.FileName;
            SourceFilename = filename;

            var language = ResourceExtentions.ExtractLanguageFromFileName(filename);
            if (language != null)
            {
                try
                {
                    SourceLanguage = Languages.FirstOrDefault(x => x.LocalizationSuffix.Contains(language));
                }
                catch(ArgumentNullException) { }
            }

            TargetDirectory = Path.GetDirectoryName(filename);
            if (TargetLanguage != null)
            {
                var targetFilename = ResourceExtentions.GetFilenameWithExtention(SourceFilename, TargetLanguage.LocalizationSuffix);
                TargetFilenameWithExtention = Path.GetFileName(targetFilename);
            }
            else
            {
                TargetFilenameWithExtention = ResourceExtentions.GetFilenameWithExtention(SourceFilename, "");
            }
        }

        public void SelectDirectoryCommand()
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.ShowNewFolderButton = true;
            if (Directory.Exists(TargetDirectory))
                dialog.SelectedPath = TargetDirectory;

            var dialogResult = dialog.ShowDialog();
            if (dialogResult == DialogResult.OK)
            {
                string path = dialog.SelectedPath;
                TargetDirectory = path;
            }
        }

        public void OpenApiHistoryCommand()
        {
            dynamic viewSettings = new ExpandoObject();
            IApiHistoryView apiHistoryViewModel = (ApiHistoryViewModel)IoC.Get<IApiHistoryView>();
            var dialogResult = _windowManager.ShowDialog(apiHistoryViewModel, settings: viewSettings) == true;
            if (dialogResult != true) 
                return;

            var history = IoC.Get<IApiHistoryView>();
            if (history != null && !string.IsNullOrEmpty(history.SelectedApiKey.ApiKey))
            {
                ApiKey = history.SelectedApiKey.ApiKey;
                try
                {
                    var findedService = EnumExtentions.GetEnumValueByDescription<LanguageService>(history.SelectedApiKey.Service);
                    Service = findedService;
                }
                catch (ArgumentException) { }
            }
        }

        public void Handle(string message)
        {
            switch (message)
            {
                case "LanguagesUpdated":
                    UpdateLanguages();
                    break;
                default:
                    break;
            }
        }

        private void UpdateLanguages()
        {
            Languages.Clear();
            var languages = _container.Languages.GetAll();
            foreach (var col in languages)
                Languages.Add(_mapper.Map<Language, LanguageModel>(col));

            if (Languages.Count > 1)
            {
                if (_sourceLanguageName != null)
                {
                    var source = Languages.FirstOrDefault(x => x.LanguageName == _sourceLanguageName);
                    if (source != null)
                        SourceLanguage = source;
                    else
                        SourceLanguage = Languages[0];
                }

                if (_targetLanguageName != null)
                {
                    var target = Languages.FirstOrDefault(x => x.LanguageName == _targetLanguageName);
                    if (target != null)
                        TargetLanguage = target;
                    else
                        TargetLanguage = Languages[1];
                }
            }
        }

        public void LoadConfig(IVisualConfig config)
        {
            ApiKey = config.ApiKey;
            Service = config.Service;

            var source = Languages.FirstOrDefault(x => x.LanguageName == config.SourceLanguage);
            if (source != null)
                SourceLanguage = source;

            var target = Languages.FirstOrDefault(x => x.LanguageName == config.TargetLanguage);
            if (target != null)
                TargetLanguage = target;
        }
    }
}
