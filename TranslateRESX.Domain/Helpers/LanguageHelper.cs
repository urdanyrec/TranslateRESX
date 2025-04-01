using System.Collections.Generic;
using TranslateRESX.Domain.Enums;

namespace TranslateRESX.Domain.Helpers
{
    public static class LanguageHelper
    {
        public static Dictionary<LanguageType, string> LanguageDictionary = new Dictionary<LanguageType, string>
        {
            { LanguageType.Russian, "ru" },
            { LanguageType.English, "en" }
        };
    }
}
