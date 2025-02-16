using Not.Application.Behinds.Adapters;
using Not.Application.RPC;
using Not.Blazor.Ports;

namespace NTS.Judge.Core.Behinds.Adapters;

public class RemoteConnectionsBehind : ObservableBehind, IConnectionsBehind
{
    List<string> ActiveConnections { get; } = [];

    protected override Task<bool> PerformInitialization(params IEnumerable<object> arguments)
    {
        var result = ActiveConnections.Any();
        return Task.FromResult(result);
    }

    public void AddConnection(string connectionId)
    {
        ActiveConnections.Add(connectionId);
        EmitChange();
    }

    public void RemoveConnection(string connectionId)
    {
        ActiveConnections.Remove(connectionId);
        EmitChange();
    }

    public int GetConnectionsCount()
    {
        return ActiveConnections.Count;
    }
}
