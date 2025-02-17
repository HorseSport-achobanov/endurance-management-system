using Not.Application.RPC.SignalR;
using Not.Blazor.Ports;
using Not.Injection;

namespace Not.Application.RPC;

public interface IConnectionsBehind : IObservableBehind
{
    void Add(string connectionId);
    void Remove(string connectionId);
    HashSet<string> RemoteConnections { get; }
    bool IsServerConnected { get; }
    RpcConnectionStatus ServerConnectionStatus { get; }
}
