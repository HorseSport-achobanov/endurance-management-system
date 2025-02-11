using Microsoft.AspNetCore.SignalR;
using NTS.ACL.Entities;
using NTS.ACL.Enums;
using NTS.ACL.Factories;
using NTS.ACL.RPC;
using NTS.Application.RPC;
using NTS.Domain.Core.Objects.Payloads;
using NTS.ACL.Factories;

using Not.Safe;

namespace NTS.Judge.MAUI.Server.RPC;

public class JudgeRpcHub : Hub<IJudgeClientProcedures> //, IJudgeHubProcedures
{
    readonly IHubContext<WitnessRpcHub, ILegacyWitnessClientProcedures> _witnessRelay;

    public JudgeRpcHub(IHubContext<WitnessRpcHub, ILegacyWitnessClientProcedures> witnessRelay)
    {
        _witnessRelay = witnessRelay;
    }

    public async Task SendParticipationEliminated(ParticipationEliminated revoked)
    {
        Task action() => SafeSendParticipationEliminated(revoked);
        await SafeHelper.Run(action);
    }

    public async Task SendParticipationRestored(ParticipationRestored restored)
    {
        Task action() => SafeSendParticipationRestored(restored);
        await SafeHelper.Run(action);
    }

    public async Task SendStartCreated(PhaseCompleted phaseCompleted)
    {
        Task action() => SafeSendStartCreated(phaseCompleted);
        await SafeHelper.Run(action);
    }

    public async Task SafeSendParticipationEliminated(ParticipationEliminated revoked)
    {
        var emsParticipation = ParticipationFactory.CreateEms(revoked.Participation);
        var entry = new EmsParticipantEntry(emsParticipation);
        await _witnessRelay.Clients.All.ReceiveEntryUpdate(entry, EmsCollectionAction.Remove);
    }

    public async Task SafeSendParticipationRestored(ParticipationRestored restored)
    {
        var emsParticipation = ParticipationFactory.CreateEms(restored.Participation);
        var entry = new EmsParticipantEntry(emsParticipation);
        await _witnessRelay.Clients.All.ReceiveEntryUpdate(entry, EmsCollectionAction.AddOrUpdate);
    }

    public async Task SafeSendStartCreated(PhaseCompleted phaseCompleted)
    {
        var emsParticipation = ParticipationFactory.CreateEms(phaseCompleted.Participation);
        var entry = new EmsStartlistEntry(emsParticipation);
        await _witnessRelay.Clients.All.ReceiveEntry(entry, EmsCollectionAction.AddOrUpdate);
    }
}
