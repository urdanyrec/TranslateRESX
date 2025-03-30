using System.Text;
using System.Text.Json;
using TranslateRESX.Core.Helpers;

namespace TranslateRESX.Core.Translators
{
    public class YandexTranslator : ITranslator
    {
        public string ApiKey { get; }

        public string DestinationLanguage { get; }

        private readonly HttpClient _httpClient;

        public YandexTranslator(string apiKey, string targetLanguage)
        {
            ApiKey = apiKey;
            DestinationLanguage = targetLanguage;
            _httpClient = new HttpClient();
        }

        public async Task<TranslateTaskResult> TranslateTextAsync(string text, string sourceLanguage)
        {
            TranslateTaskResult taskResult = new TranslateTaskResult();
            if (string.IsNullOrWhiteSpace(text))
                return null;

            var requestData = new
            {
                sourceLanguageCode = sourceLanguage,
                targetLanguageCode = DestinationLanguage,
                texts = new[] { text },
                folderId = ""
            };
            string requestJson = JsonSerializer.Serialize(requestData);
            var content = new StringContent(requestJson, Encoding.UTF8, "application/json");
            string url = $"https://translate.api.cloud.yandex.net/translate/v2/translate?key={ApiKey}";

            var response = await _httpClient.PostAsync(url, content);
            taskResult.Request = requestJson;
            taskResult.StatusCode = (int)response.StatusCode;
            if (response.IsSuccessStatusCode)
            {
                string responseJson = await response.Content.ReadAsStringAsync();
                using JsonDocument doc = JsonDocument.Parse(responseJson);
                var translatedText = doc.RootElement
                    .GetProperty("translations")[0]
                    .GetProperty("text")
                    .GetString();
      
                taskResult.Answer = responseJson;
                taskResult.TranslatedPhrase = translatedText;
            }
            
            return taskResult;
        }
    }
}