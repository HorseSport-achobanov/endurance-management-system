using MudBlazor;
using Not.Application.RPC;
using Not.Application.RPC.SignalR;
using Not.Notify;


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

    protected override async Task OnInitializedAsync()
    {
        await Observe(ConnectionsCount);
        _isConnected = RpcSocket.IsConnected;
        RpcSocket.Error += HandleRpcErrors;
        RpcSocket.ServerConnectionChanged += HandleServerConnectionChanged;
    }


    async void HandleRpcErrors(object? sender, RpcError rpcError)
    {
        _rpcConnectionStatus = RpcConnectionStatus.Disconnected;
        NotifyHelper.Error(rpcError.Exception);
        await InvokeAsync(StateHasChanged);
    }

    async void HandleServerConnectionChanged(object? sender, RpcConnectionStatus e )
    {
        _rpcConnectionStatus = e;
        _isConnected = _rpcConnectionStatus.Equals(RpcConnectionStatus.Connected);
        if (_rpcConnectionStatus == RpcConnectionStatus.Disconnected)
        {
            SpinnerColor = Color.Error;
        } else { 
            SpinnerColor = Color.Warning;
        }
        await InvokeAsync(StateHasChanged);
    }
}
