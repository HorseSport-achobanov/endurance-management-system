using Not.Injection;

namespace Not.Application.RPC;

public interface IConnectionsCounter : ISingleton
{
    List<string> ActiveConnections { get; }
    void AddConnection(string connectionId);
    void RemoveConnection(string connectionId);
    int GetConnectionsCount();
}
