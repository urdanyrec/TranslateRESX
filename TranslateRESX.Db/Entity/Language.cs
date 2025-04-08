using System;
using TranslateRESX.DB.Entity;

namespace TranslateRESX.Db.Entity
{
    [Serializable]
    public class Language : IEntity
    {
        public int Id { get; set; }

        public string LanguageName { get; set; }

        public string LocalizationSuffix { get; set; }

        public string LanguageCode { get; set; }

        public bool IsDefault { get; set; }
    }
}
