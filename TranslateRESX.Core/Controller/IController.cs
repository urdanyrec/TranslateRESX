using TranslateRESX.DB;
using TranslateRESX.Domain.Models;

namespace TranslateRESX.Core.Controller
{
    public interface IController
    {
        CancellationTokenSource CancellationTokenSource { get; }
        Task Start(IParametersModel parameters, IContainer dbContainer, IState state, CancellationToken cancellationToken);
        event EventHandler Finished;
        event EventHandler Started;
    }
}
