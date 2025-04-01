using System.ComponentModel;

namespace TranslateRESX.Domain.Enums
{
    public enum StateType
    {
        [Description("Не запущено")]
        NotRunning,

        [Description("Выполняется")]
        Processing,

        [Description("Выполнено")]
        Completed,

        [Description("Отменено")]
        Canceled,

        [Description("Ошибка")]
        Error
    }
}
