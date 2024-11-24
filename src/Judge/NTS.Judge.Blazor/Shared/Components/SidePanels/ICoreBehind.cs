using Not.Blazor.Ports;
using Not.Injection;

namespace NTS.Judge.Blazor.Shared.Components.SidePanels;

public interface ICoreBehind : IObservableBehind, ISingleton
{
    bool IsStarted { get; }
    Task Import(string contents);
    Task Start();
}
