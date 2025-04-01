using System.Net.Http.Headers;
using System.Text;
using TranslateRESX.Core.Translators;

namespace TranslateRESX.Test
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var translator = new YandexTranslator("b1gpmia6maqtfip0a20d", "en");
            var test = translator.TranslateTextAsync("Тест перевода текста через яндекс", "ru");
            var text = test.Result.Answer;
        }
    }
}