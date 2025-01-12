﻿using Microsoft.AspNetCore.SignalR;
using NTS.ACL.Entities;
using NTS.ACL.Enums;
using NTS.ACL.Factories;
using NTS.ACL.RPC;
using NTS.Application.RPC;
using NTS.Domain.Core.Objects.Payloads;
using NTS.Domain.Objects;

namespace NTS.Judge.MAUI.Server.RPC;

public class JudgeRpcHub : Hub<IJudgeClientProcedures>, IJudgeHubProcedures
{
    readonly IHubContext<WitnessRpcHub, IWitnessClientProcedures> _witnessRelay;

    public JudgeRpcHub(IHubContext<WitnessRpcHub, IWitnessClientProcedures> witnessRelay)
    {
        _witnessRelay = witnessRelay;
    }

    public async Task SendParticipationEliminated(ParticipationEliminated revoked)
    {
        var emsParticipation = ParticipationFactory.CreateEms(revoked.Participation);
        var entry = new EmsParticipantEntry(emsParticipation);
        await _witnessRelay.Clients.All.ReceiveEntryUpdate(entry, EmsCollectionAction.Remove);
    }

    public async Task SendParticipationRestored(ParticipationRestored restored)
    {
        var emsParticipation = ParticipationFactory.CreateEms(restored.Participation);
        var entry = new EmsParticipantEntry(emsParticipation);
        await _witnessRelay.Clients.All.ReceiveEntryUpdate(entry, EmsCollectionAction.AddOrUpdate);
    }

    public async Task SendStartCreated(PhaseCompleted phaseCompleted)
    {
        var emsParticipation = ParticipationFactory.CreateEms(phaseCompleted.Participation);
        var entry = new EmsStartlistEntry(emsParticipation);
        await _witnessRelay.Clients.All.ReceiveEntry(entry, EmsCollectionAction.AddOrUpdate);
    }
}
