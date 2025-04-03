using Caliburn.Micro;
using Microsoft.Win32;
using System;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using TranslateRESX.ApiHistory;
using TranslateRESX.Core.Helpers;
using TranslateRESX.Domain.Enums;

namespace TranslateRESX.TranslateParameters
{
    public class TranslateParametersViewModel : PropertyChangedBase, ITranslateParametersView
    {
        private readonly IWindowManager _windowManager;

        public TranslateParametersViewModel(IWindowManager windowManager)
        {
            _windowManager = windowManager;
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

        private LanguageType _sourceLanguage = LanguageType.Russian;
        public LanguageType SourceLanguage
        {
            get => _sourceLanguage;
            set
            {
                _sourceLanguage = value;
                NotifyOfPropertyChange();
            }
        }

        private LanguageType _targetLanguage = LanguageType.English;
        public LanguageType TargetLanguage
        {
            get => _targetLanguage;
            set
            {
                if (value == SourceLanguage)
                    return;

                _targetLanguage = value;
                NotifyOfPropertyChange();

                if (string.IsNullOrEmpty(TargetFilenameWithExtention))
                    return;

                var targetLanguage = ResourceExtentions.LanguageDictionary[TargetLanguage];
                var targetFilename = ResourceExtentions.GetFilenameWithExtention(TargetFilenameWithExtention, targetLanguage.Item1);
                TargetFilenameWithExtention = Path.GetFileName(targetFilename);
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
                    SourceLanguage = ResourceExtentions.LanguageDictionary.FirstOrDefault(x => x.Value.Item1.Contains(language)).Key;
                }
                catch(ArgumentNullException) { }
            }

            var targetLanguage = ResourceExtentions.LanguageDictionary[TargetLanguage];
            var targetFilename = ResourceExtentions.GetFilenameWithExtention(SourceFilename, targetLanguage.Item1);

            TargetDirectory = Path.GetDirectoryName(filename);
            TargetFilenameWithExtention = Path.GetFileName(targetFilename);
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
    }
}
