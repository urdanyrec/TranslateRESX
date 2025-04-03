using System.IO;
using System;
using TranslateRESX.Domain.Enums;
using System.Reflection;
using Newtonsoft.Json;
using System.Runtime;

namespace TranslateRESX.Domain.Models
{
    [Serializable]
    public class VisualConfig : IVisualConfig
    {
        public string ApiKey { get; set; }

        public LanguageService Service { get; set; } = LanguageService.Yandex;

        public string SourceFilename { get; set; }

        public string TargetFilename { get; set; }

        public LanguageType SourceLanguage { get; set; } = LanguageType.Russian;

        public LanguageType TargetLanguage { get; set; } = LanguageType.English;

        [JsonIgnore]
        public string DataPath { get; set; }

        private string _filename = "VisualConfig.json";

        public VisualConfig()
        {
            DataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Assembly.GetEntryAssembly().GetName().Name, "Data");
            if (!Directory.Exists(DataPath))
                Directory.CreateDirectory(DataPath);
        }

        public bool Read()
        {
            try
            {
                if (!Directory.Exists(DataPath))
                {
                    Directory.CreateDirectory(DataPath);
                    CreateDefault();
                }

                using (TextReader tr = new StreamReader(Path.Combine(DataPath, _filename)))
                {
                    var data = tr.ReadToEnd();
                    var jsonSerializerSettings = new JsonSerializerSettings();
                    jsonSerializerSettings.Formatting = Formatting.Indented;
                    var settings = JsonConvert.DeserializeObject<VisualConfig>(data, jsonSerializerSettings);

                    ApiKey = settings.ApiKey;
                    Service = settings.Service;
                    SourceFilename = settings.SourceFilename;
                    TargetFilename = settings.TargetFilename;
                    SourceLanguage = settings.SourceLanguage;
                    TargetLanguage = settings.TargetLanguage;
                }

                return true;
            }
            catch (Exception)
            {
                CreateDefault();
                throw new ApplicationException($"Ошибка чтения настроек: {_filename}. Созданы настройки по умолчанию");
            }
        }

        public bool Write()
        {
            try
            {
                if (!Directory.Exists(DataPath))
                    Directory.CreateDirectory(DataPath);

                var jsonSerializerSettings = new JsonSerializerSettings();
                jsonSerializerSettings.Formatting = Formatting.Indented;
                var json = JsonConvert.SerializeObject(this, jsonSerializerSettings);
                File.WriteAllText(Path.Combine(DataPath, _filename), json);
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Ошибка сохранения настроек {_filename}", ex);
            }
            return true;
        }

        public void CreateDefault()
        {
            var settings = new VisualConfig();
            var jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.Formatting = Formatting.Indented;
            var json = JsonConvert.SerializeObject(settings, jsonSerializerSettings);
            File.WriteAllText(Path.Combine(DataPath, _filename), json);
        }
    }
}
