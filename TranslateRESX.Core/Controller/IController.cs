using System;
using System.Threading;
using System.Threading.Tasks;
using TranslateRESX.Core.Events;
using TranslateRESX.DB;
using TranslateRESX.Domain.Models;

namespace TranslateRESX.Core.Controller
{
    public interface IController
    {
        CancellationTokenSource CancellationTokenSource { get; set; }

        event EventHandler<StateChangedEventArgs> StateChanged;

        Task Start(IParametersModel parameters, IContainer dbContainer);      
    }
}
