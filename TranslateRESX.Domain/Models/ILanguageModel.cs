namespace TranslateRESX.Domain.Models
{
    public interface ILanguageModel
    {
        int Id { get; set; }

        string LanguageName { get; set; }

        string LocalizationSuffix { get; set; }

        string LanguageCode { get; set; }

        bool IsDefault { get; set; }

    }
}
