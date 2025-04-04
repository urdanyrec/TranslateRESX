using System.Threading.Tasks;
using TranslateRESX.Core.Helpers;

namespace TranslateRESX.Core.Translators
{
    /// <summary>
    /// Эмулятор сервиса перевода для отладки
    /// </summary>
    public class Emulator : ITranslator
    {
        public string ApiKey { get; }

        public string TargetLanguageCode { get; }

        public Emulator(string apiKey, string targetLanguage)
        {
            ApiKey = apiKey;
            TargetLanguageCode = targetLanguage;
        }

        public async Task<TranslateTaskResult> TranslateTextAsync(string text, string sourceLanguage)
        {
            TranslateTaskResult taskResult = new TranslateTaskResult();
            taskResult.StatusCode = 200;
            taskResult.WaitTime = 0;
            taskResult.TranslatedText = "text";
            return taskResult;
        }
    }
}
