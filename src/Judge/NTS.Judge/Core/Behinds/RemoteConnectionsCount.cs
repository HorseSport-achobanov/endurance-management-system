using Not.Application.RPC;

namespace NTS.Judge.Core.Behinds;

public class RemoteConnectionsCount : IConnectionsCounter
{
    public List<string> ActiveConnections { get; } = [];

    public void AddConnection(string connectionId)
    {
        ActiveConnections.Add(connectionId);
    }

    public void RemoveConnection(string connectionId)
    {
        ActiveConnections.Remove(connectionId);
    }

    public int GetConnectionsCount()
    {
        return ActiveConnections.Count;
    }
}
