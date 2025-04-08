using TranslateRESX.Domain.Enums;

namespace TranslateRESX.Domain.Models
{
    public interface IParametersModel
    {
        /// <summary>
        /// Ключ API 
        /// </summary>
        string ApiKey { get; set; }

        /// <summary>
        /// Переводчик
        /// </summary>
        LanguageService Service { get; set; }

        /// <summary>
        /// Путь к исходному файлу ресурсов
        /// </summary>
        string SourceFilename { get; set; }

        /// <summary>
        /// Путь к новому файлу ресурсов
        /// </summary>
        string TargetFilename { get; }

        /// <summary>
        /// Исходный язык файла ресурсов
        /// </summary>
        LanguageModel SourceLanguage { get; set; }

        /// <summary>
        /// Язык перевлда
        /// </summary>
        LanguageModel TargetLanguage { get; set; }

        /// <summary>
        /// Перезаписать все ключи (для уже существующего автосгенерированного файла ресурсов)
        /// </summary>
        bool RewriteAllKeys { get; set; }
    }
}
