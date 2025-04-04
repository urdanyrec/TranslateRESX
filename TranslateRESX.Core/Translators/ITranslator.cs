using System.Threading.Tasks;
using TranslateRESX.Core.Helpers;

namespace TranslateRESX.Core.Translators
{
    public interface ITranslator
    {
        string ApiKey { get; }

        string TargetLanguageCode { get; }

        Task<TranslateTaskResult> TranslateTextAsync(string text, string sourceLanguage);
    }
}
