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
        /// Исходный язык файла ресурсов
        /// </summary>
        LanguageType SourceLanguage { get; set; }

        /// <summary>
        /// Язык перевлда
        /// </summary>
        LanguageType TargetLanguage { get; set; }
    }
}
