using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using TranslateRESX.Core.Helpers;

namespace TranslateRESX.Core.Translators
{
    /// <summary>
    /// Сервис перевода через Yandex API
    /// </summary>
    public class DeepLTranslator : ITranslator
    {
        public string ApiKey { get; }

        public string TargetLanguageCode { get; }

        private readonly HttpClient _httpClient;

        public DeepLTranslator(string apiKey, string targetLanguage)
        {
            ApiKey = apiKey;
            TargetLanguageCode = targetLanguage;
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        public async Task<TranslateTaskResult> TranslateTextAsync(string text, string sourceLanguage)
        {
            TranslateTaskResult taskResult = new TranslateTaskResult();
            if (string.IsNullOrWhiteSpace(text))
                return null;

            string requestText = text;
            var requestData = new
            {
                target_lang = TargetLanguageCode,
                text = requestText,
            };
            string requestJson = JsonSerializer.Serialize(requestData);

            HttpRequestMessage request = new HttpRequestMessage();
            request.Method = HttpMethod.Post;
            request.RequestUri = new Uri("https://api.deepl.com/v2/translate");
            request.Content = new StringContent(requestJson, Encoding.UTF8, "application/json");
            request.Headers.Authorization = new AuthenticationHeaderValue("DeepL-Auth-Key", ApiKey);

            var stopwatch = Stopwatch.StartNew();
            var response = await _httpClient.SendAsync(request);
            stopwatch.Stop();

            taskResult.WaitTime = stopwatch.ElapsedMilliseconds;
            taskResult.Request = requestJson;
            taskResult.StatusCode = (int)response.StatusCode;
            if (response.IsSuccessStatusCode)
            {
                string responseJson = await response.Content.ReadAsStringAsync();
                JsonDocument doc = JsonDocument.Parse(responseJson);
                var translatedText = doc.RootElement.GetProperty("text").GetString();
                taskResult.Answer = responseJson;
                taskResult.TranslatedText = translatedText;
            }

            return taskResult;
        }
    }
}
