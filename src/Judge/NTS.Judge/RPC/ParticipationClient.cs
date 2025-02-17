using Not.Application.RPC;
using Not.Application.RPC.Clients;
using Not.Application.RPC.SignalR;
using Not.Injection;
using Not.Startup;
using NTS.Application.RPC;
using NTS.Domain.Core.Aggregates;
using NTS.Domain.Core.Objects.Payloads;
using NTS.Domain.Objects;
using NTS.Judge.Core;

namespace NTS.Judge.RPC;

public class ParticipationClient : RpcClient, IParticipationRpcClient, IStartupInitializer
{
    readonly ISnapshotProcessor _snapshotProcessor;
    readonly IConnectionsBehind _remoteConnections;

    public ParticipationClient(
        IRpcSocket socket,
        ISnapshotProcessor snapshotProcessor,
        IConnectionsBehind remoteConnections
    )
        : base(socket)
    {
        _snapshotProcessor = snapshotProcessor;
        _remoteConnections = remoteConnections;
        RegisterClientProcedure<IEnumerable<Snapshot>>(
            nameof(IJudgeClientProcedures.ReceiveSnapshots),
            ReceiveSnapshots
        );
    }

    public void RunAtStartup()
    {
        Participation.PHASE_COMPLETED_EVENT.Subscribe(SendStartCreated);
        Participation.ELIMINATED_EVENT.Subscribe(SendParticipationEliminated);
        Participation.RESTORED_EVENT.Subscribe(SendParticipationRestored);
    }

    public async Task ReceiveSnapshots(IEnumerable<Snapshot> snapshots)
    {
        foreach (Snapshot snapshot in snapshots)
        {
            await _snapshotProcessor.Process(snapshot);
        }
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

    public async Task SendParticipationEliminated(ParticipationEliminated revoked)
    {
        await InvokeHubProcedure(nameof(IJudgeHubProcedures.SendParticipationEliminated), revoked);
    }

    public async Task SendParticipationRestored(ParticipationRestored restored)
    {
        await InvokeHubProcedure(nameof(IJudgeHubProcedures.SendParticipationRestored), restored);
    }

    public async Task SendStartCreated(PhaseCompleted phaseCompleted)
    {
        await InvokeHubProcedure(nameof(IJudgeHubProcedures.SendStartCreated), phaseCompleted);
    }
}

public interface IParticipationRpcClient
    : IParticipationClientProcedures,
        IRpcClient,
        ITransient { }
