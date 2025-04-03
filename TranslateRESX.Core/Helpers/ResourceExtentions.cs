using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using TranslateRESX.Domain.Enums;

namespace TranslateRESX.Core.Helpers
{
    public static class ResourceExtentions
    {
        public static Dictionary<LanguageType, (string, string)> LanguageDictionary { get; } = new Dictionary<LanguageType, (string, string)>
        {
            { LanguageType.Russian, ("ru-RU", "ru") },
            { LanguageType.English, ("en-US", "en") },
            { LanguageType.French,  ("fr-FR", "fr") },
            { LanguageType.Spanish, ("es-ES", "es") }
        };

        /// <summary>
        /// Формирует путь к файлу с нужным языковым кодом в расширении
        /// </summary>
        /// <param name="fullPath">Полный путь к файлу</param>
        /// <param name="targetLanguage">Языковой код</param>
        /// <returns></returns>
        public static string GetFilenameWithExtention(string fullPath, string targetLanguage)
        {
            string fileName = Path.GetFileNameWithoutExtension(fullPath);
            string directory = Path.GetDirectoryName(fullPath);
            string extension = Path.GetExtension(fullPath);

            string targetFilename;
            var langMatch = Regex.Match(fileName, @"(\.[a-z]{2,3}(?:-[a-z]{2,4})?)$", RegexOptions.IgnoreCase);
            if (langMatch.Success)
            {
                // Полностью заменяем ВЕСЬ языковой суффикс (включая точку) на новый
                string newFileName = fileName.Substring(0, langMatch.Groups[1].Index) + $".{targetLanguage}";
                targetFilename = Path.Combine(directory, newFileName + extension);
            }
            else
            {
                // Если языкового суффикса нет, просто добавляем новый
                targetFilename = Path.Combine(directory, $"{fileName}.{targetLanguage}{extension}");
            }

            return targetFilename;
        }

        /// <summary>
        /// Извлекает языковой код из имени файла ресурсов в формате [name].[lang].resx
        /// </summary>
        /// <param name="fullPath">Полный путь к файлу</param>
        /// <returns>Языковой код или null, если код отсутствует</returns>
        public static string ExtractLanguageFromFileName(string fullPath)
        {
            string fileName = Path.GetFileNameWithoutExtension(fullPath);
            string[] parts = fileName.Split('.');
            if (parts.Length < 2)
                return null;

            string langPart = parts[parts.Length - 1];
            string langCode = langPart.Split('-')[0];

            // Проверяем, что это валидный код языка (2-3 буквы)
            if (Regex.IsMatch(langCode, @"^[a-z]{2,3}$", RegexOptions.IgnoreCase))
            {
                return langCode.ToLowerInvariant();
            }

            return null;
        }
    }
}
