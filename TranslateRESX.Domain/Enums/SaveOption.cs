using System.ComponentModel;

namespace TranslateRESX.Domain.Enums
{
    public enum SaveOption
    {
        [Description("Создать новый файл")]
        CreateNew,

        [Description("Добавить в существующий")]
        AddToExist
    }
}
