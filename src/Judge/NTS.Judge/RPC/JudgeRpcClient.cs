using Not.Application.RPC.Clients;
using Not.Application.RPC.SignalR;
using Not.Injection;
using NTS.Application.RPC;
using NTS.Domain.Core.Objects.Payloads;
using NTS.Domain.Objects;
using NTS.Judge.Core;

namespace NTS.Judge.RPC;

public class JudgeRpcClient : RpcClient, IJudgeRpcClient
{
    readonly ISnapshotProcessor _snapshotProcessor;

    public JudgeRpcClient(IRpcSocket socket, ISnapshotProcessor snapshotProcessor)
        : base(socket)
    {
        _snapshotProcessor = snapshotProcessor;
        RegisterClientProcedure<IEnumerable<Snapshot>>(nameof(IJudgeClientProcedures.ReceiveSnapshots), ReceiveSnapshots);
    }

    public async Task ReceiveSnapshots(IEnumerable<Snapshot> snapshots)
    {
        foreach (Snapshot snapshot in snapshots)
        {
            await _snapshotProcessor.Process(snapshot);
        }
    }

    public async Task SendParticipationEliminated(ParticipationEliminated revoked)
    {
        await InvokeHubProcedure(nameof(IJudgeHubProcedures.SendParticipationEliminated), revoked);
    }

    public async Task SendParticipationRestored(ParticipationRestored restored)
    {
        await InvokeHubProcedure(nameof(IJudgeHubProcedures), restored);
    }

    public async Task SendStartCreated(PhaseCompleted phaseCompleted)
    {
        await InvokeHubProcedure(nameof(IJudgeHubProcedures), phaseCompleted);
    }
}

public interface IJudgeRpcClient : IJudgeHubProcedures, IJudgeClientProcedures, IRpcClient, ITransient
{
}
