using TranslateRESX.Domain.Enums;

namespace TranslateRESX.Domain.Models
{
    public class ParametersModel : IParametersModel
    {
        public string ApiKey { get; set; }

        public LanguageService Service { get; set; }

        public string SourceFilename { get; set; }

        public string TargetFilename { get; set; }

        public LanguageModel SourceLanguage { get; set; }

        public LanguageModel TargetLanguage { get; set; }

        public bool RewriteAllKeys { get; set; }
    }
}
