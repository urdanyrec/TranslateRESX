using System;
using TranslateRESX.Domain.Models;

namespace TranslateRESX.Core.Events
{
    public class StateChangedEventArgs : EventArgs
    {
        public IState CurrentState { get; set; }


        public StateChangedEventArgs(IState state)
        {
            CurrentState = state;
        }
    }
}
