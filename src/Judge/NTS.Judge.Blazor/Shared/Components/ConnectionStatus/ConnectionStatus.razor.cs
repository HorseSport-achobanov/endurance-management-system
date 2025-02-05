using MudBlazor;
using Not.Application.RPC;
using Not.Application.RPC.SignalR;


namespace NTS.Judge.Blazor.Shared.Components.ConnectionStatus;

public partial class ConnectionStatus
{
    RpcConnectionStatus _rpcConnectionStatus;
    bool _isConnected;
    Color SpinnerColor { get; set; } = Color.Error;

    [Inject]
    IRpcSocket RpcSocket { get; set; } = default!;

    [Inject]
    IConnectionsCounter ConnectionsCount { get; set; } = default!;

    protected override void OnInitialized()
    {
        RpcSocket.Error += HandleRpcErrors;
        RpcSocket.ServerConnectionChanged += HandleServerConnectionChanged;
        RpcSocket.ServerConnectionInfo += HandleServerConnectionInfo;
    }

    void HandleRpcErrors(object? sender, RpcError rpcError)
    {
        _isConnected = _rpcConnectionStatus.Equals(RpcConnectionStatus.Connected);
    }

    void HandleServerConnectionChanged(object? sender, RpcConnectionStatus e )
    {
        _rpcConnectionStatus = e;
        _isConnected = _rpcConnectionStatus.Equals(RpcConnectionStatus.Connected);
        if (_rpcConnectionStatus == RpcConnectionStatus.Disconnected)
        {
            SpinnerColor = Color.Error;
        } else { 
            SpinnerColor = Color.Warning;
        }
        StateHasChanged();
    }

    void HandleServerConnectionInfo(object? sender, string e) 
    {
        _isConnected = true;
        StateHasChanged();
    }
}
