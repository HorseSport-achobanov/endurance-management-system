using Not.Blazor.Ports;
using Not.Injection;

namespace Not.Application.RPC;

public interface IConnectionsCounter : IObservableBehind
{
    void AddConnection(string connectionId);
    void RemoveConnection(string connectionId);
    int GetConnectionsCount();
}
