using System;

namespace TranslateRESX.Domain.Models
{
    public class LanguageModel : ILanguageModel
    {
        public int Id { get; set; }

        public string LanguageName { get; set; }

        public string LocalizationSuffix { get; set; }

        public string LanguageCode { get; set; }

        public bool IsDefault { get; set; }
    }
}
