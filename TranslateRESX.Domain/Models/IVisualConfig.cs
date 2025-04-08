using Newtonsoft.Json;
using TranslateRESX.Domain.Enums;

namespace TranslateRESX.Domain.Models
{
    public interface IVisualConfig : IConfig
    {
        string ApiKey { get; set; }

        LanguageService Service { get; set; }

        string SourceFilename { get; set; }

        string TargetFilename { get; }

        string SourceLanguage { get; set; }

        string TargetLanguage { get; set; }

        bool RewriteAllKeys { get; set; }

        [JsonIgnore]
        string DataPath { get; }
    }
}
