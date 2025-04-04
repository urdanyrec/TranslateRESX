using System.Diagnostics;
using System.Threading.Tasks;
using TranslateRESX.Core.Helpers;
using Google.Cloud.Translation.V2;

namespace TranslateRESX.Core.Translators
{
    /// <summary>
    /// Сервис перевода через Google API
    /// </summary>
    public class GoogleTranslator : ITranslator
    {
        public string ApiKey { get; }

        public string TargetLanguageCode { get; }

        private readonly TranslationClient _translationClient ;

        public GoogleTranslator(string apiKey, string targetLanguage)
        {
            ApiKey = apiKey;
            TargetLanguageCode = targetLanguage;
            _translationClient = TranslationClient.CreateFromApiKey(apiKey);
        }

        public async Task<TranslateTaskResult> TranslateTextAsync(string text, string sourceLanguage)
        {
            TranslateTaskResult taskResult = new TranslateTaskResult();
            if (string.IsNullOrWhiteSpace(text))
                return null;

            try
            {
                var stopwatch = Stopwatch.StartNew();
                var response = _translationClient.TranslateText(text, TargetLanguageCode);
                stopwatch.Stop();

                taskResult.WaitTime = stopwatch.ElapsedMilliseconds;
                taskResult.Request = text;
                taskResult.StatusCode = 200;
                taskResult.Answer = response.TranslatedText;
                taskResult.TranslatedText = response.TranslatedText;
            }
            catch
            {
                taskResult.StatusCode = 400;
            }

            return taskResult;
        }
    }
}
