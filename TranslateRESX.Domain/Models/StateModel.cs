using TranslateRESX.Domain.Enums;

namespace TranslateRESX.Domain.Models
{
    public class StateModel : IState
    {
        public StateType State { get; set; }

        public int CurrentIndex { get; set; }

        public int AllCount { get; set; }

        public double Progress { get; set; }

        public string Log { get; set; }
    }
}
