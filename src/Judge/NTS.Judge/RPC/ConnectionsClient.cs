using Not.Application.RPC;
using Not.Application.RPC.Clients;
using Not.Application.RPC.SignalR;
using Not.Injection;
using NTS.Application.RPC;

namespace NTS.Judge.RPC;

public class ConnectionsClient : RpcClient, IConnectionsRpcClient
{
    readonly IConnectionsBehind _remoteConnections;

    public ConnectionsClient(IRpcSocket socket, IConnectionsBehind remoteConnections)
        : base(socket)
    {
        _remoteConnections = remoteConnections;
        RegisterClientProcedure<string>(
            nameof(IJudgeClientProcedures.ReceiveRemoteConnectionId),
            ReceiveRemoteConnectionId
        );
        RegisterClientProcedure<string>(
            nameof(IJudgeClientProcedures.ReceiveRemoteDisconnectId),
            ReceiveRemoteDisconnectId
        );
    }

    public Task ReceiveRemoteConnectionId(string connectionId)
    {
        _remoteConnections.Add(connectionId);
        return Task.CompletedTask;
    }

    public Task ReceiveRemoteDisconnectId(string connectionId)
    {
        _remoteConnections.Remove(connectionId);
        return Task.CompletedTask;
    }
}

public interface IConnectionsRpcClient : IConnectionsClientProcedures, IRpcClient, ITransient { }
