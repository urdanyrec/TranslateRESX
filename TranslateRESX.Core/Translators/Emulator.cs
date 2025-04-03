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
    /// Эмулятор сервиса перевода для отладки
    /// </summary>
    public class Emulator : ITranslator
    {
        public string ApiKey { get; }

        public string DestinationLanguage { get; }

        public Emulator(string apiKey, string targetLanguage)
        {
            ApiKey = apiKey;
            DestinationLanguage = targetLanguage;
        }

        public async Task<TranslateTaskResult> TranslateTextAsync(string text, string sourceLanguage)
        {
            TranslateTaskResult taskResult = new TranslateTaskResult();
            taskResult.StatusCode = 200;
            taskResult.WaitTime = 0;
            taskResult.Answer = text;
            return taskResult;
        }
    }
}
