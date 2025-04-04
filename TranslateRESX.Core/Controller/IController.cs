using System;
using System.Threading;
using System.Threading.Tasks;
using TranslateRESX.Core.Events;
using TranslateRESX.Core.Helpers;
using TranslateRESX.DB;
using TranslateRESX.Domain.Models;

namespace TranslateRESX.Core.Controller
{
    public interface IController
    {
        event EventHandler<StateChangedEventArgs> StateChanged;

        Task<ControllerTaskResult> Start(IParametersModel parameters, IContainer dbContainer, CancellationToken cancellationToken);      
    }
}
