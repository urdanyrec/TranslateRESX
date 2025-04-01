using System.Collections.Generic;
using TranslateRESX.Domain.Enums;

namespace TranslateRESX.Domain.Models
{
    public interface IState
    {
        StateType State { get; set; }

        /// <summary>
        /// Номер текущего ключа
        /// </summary>
        int CurrentIndex { get; set; }

        /// <summary>
        /// Количество ключей в словаре
        /// </summary>
        int AllCount { get; set; }

        /// <summary>
        /// Лог выполнения
        /// </summary>
        string Log { get; set; }
    }
}
