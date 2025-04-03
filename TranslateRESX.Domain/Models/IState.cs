using System.Collections.Generic;
using TranslateRESX.Domain.Enums;

namespace TranslateRESX.Domain.Models
{
    public interface IState
    {
        /// <summary>
        /// Состояние
        /// </summary>
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
        /// [Количество ключей]/[текущий индекс]*100
        /// </summary>
        double Progress { get; set; }

        /// <summary>
        /// Лог выполнения
        /// </summary>
        string Log { get; set; }
    }
}
