using System.ComponentModel;

namespace TranslateRESX.Domain.Enums
{
    public enum LanguageService
    {
        [Description("Яндекс")]
        Yandex,

        [Description("Google")]
        Google,

        [Description("DeepL")]
        DeepL,

        [Description("Эмулятор")]
        Emulator
    }
}
