using TranslateRESX.Domain.Enums;

namespace TranslateRESX.Domain.Models
{
    public class ParametersModel : IParametersModel
    {
        public string ApiKey { get; set; }

        public LanguageService Service { get; set; }

        public string SourceFilename { get; set; }

        public string TargetFilename { get; set; }

        public LanguageType SourceLanguage { get; set; }

        public LanguageType TargetLanguage { get; set; }

        public bool RewriteAllKeys { get; set; }
    }
}
