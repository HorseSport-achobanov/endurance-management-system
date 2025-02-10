﻿using Microsoft.AspNetCore.SignalR;
using Not.Application.CRUD.Ports;
using Not.Concurrency.Extensions;
using Not.Safe;
using NTS.ACL.Entities.EMS;
using NTS.ACL.Entities;
using NTS.ACL.RPC;
using NTS.Application.RPC;
using NTS.ACL.Factories;
using NTS.Domain.Core.Aggregates;
using NTS.Domain.Objects;
using NTS.ACL.RPC.Procedures;
using Not.Logging;
using Not.Exceptions;

namespace NTS.Judge.MAUI.Server.RPC;

public class WitnessRpcHub : Hub<ILegacyWitnessClientProcedures>, IEmsStartlistHubProcedures
{
    readonly IRead<Participation> _participations;
    readonly IRead<EnduranceEvent> _events;
    readonly IHubContext<JudgeRpcHub, IJudgeClientProcedures> _judgeRelay;

    public WitnessRpcHub(IRead<Participation> participations, IRead<EnduranceEvent> events, IHubContext<JudgeRpcHub, IJudgeClientProcedures> judgeRelay)
    {
        _participations = participations;
        _events = events;
        _judgeRelay = judgeRelay;
    }

    public override async Task OnConnectedAsync()
    {
        try
        {
            var connectionId = base.Context.ConnectionId;
            await _judgeRelay.Clients.All.IncrementConnectionCount(connectionId);

        }
        catch (HubException ex) 
        {
            LoggingHelper.Error(ex.Message);
        }
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        try 
        { 
            var connectionId = base.Context.ConnectionId;
            await _judgeRelay.Clients.All.DecrementConnectionCount(connectionId);
        }
        catch (HubException ex) 
        {
            LoggingHelper.Error(ex.Message);
        }
    }

    public Dictionary<int, EmsStartlist> SendStartlist()
    {
        Dictionary<int, EmsStartlist> action() => SafeSendStartlist();
        var result = SafeHelper.Run(action);
        GuardHelper.ThrowIfDefault(result);
        return result;
    }

    public async Task<EmsParticipantsPayload> SendParticipants()
    {
        Task<EmsParticipantsPayload> action() => SafeSendParticipants();
        var result = await SafeHelper.Run(action);
        GuardHelper.ThrowIfDefault(result);
        return result;
    }

    public async Task ReceiveWitnessEvent(
        IEnumerable<EmsParticipantEntry> entries,
        EmsWitnessEventType type
    )
    {
        // Task.Run because Event handling in dotnet seems to hold the current thread. Further investigation is needed
        // but what was happening is that Witness apps didn't receive rpc response until the handling thread was finished
        // which is mostly visible when it causes a validation (popup) which blocks the thread until closed in Prism/WPF
        await SafeHelper.RunAsync(async () =>
        {
            var snapshots = new List<Snapshot>();
            foreach (var entry in entries)
            {
                var participation = await _participations.Read(x =>
                    x.Combination.Number == int.Parse(entry.Number)
                );
                if (participation == null)
                {
                    continue;
                }
                var isFinal = participation
                    .Phases.Take(participation.Phases.Count - 1)
                    .All(x => x.IsComplete());
                var snapshot = SnapshotFactory.Create(entry, type, isFinal);
                snapshots.Add(snapshot);
            }
            await _judgeRelay.Clients.All.ReceiveSnapshots(snapshots);
        });
    }

    public Dictionary<int, EmsStartlist> SafeSendStartlist()
    {
        var participations = _participations.ReadAll(x => !x.IsEliminated()).Result;
        var emsParticipations = participations.Select(ParticipationFactory.CreateEms);

        var startlists = new Dictionary<int, EmsStartlist>();
        foreach (var emsParticipation in emsParticipations)
        {
            var toSkip = 0;
            foreach (
                var record in emsParticipation.Participant.LapRecords.Where(x =>
                    x.Result == null || !x.Result.IsNotQualified
                )
            )
            {
                var entry = new EmsStartlistEntry(emsParticipation, toSkip);
                if (startlists.ContainsKey(entry.Stage))
                {
                    startlists[entry.Stage].Add(entry);
                }
                else
                {
                    startlists.Add(entry.Stage, new EmsStartlist([entry]));
                }
                // If record is complete, but is not last -> insert another record for current stage
                // This bullshit happens because "current" stage is not yet created. Its only created at Arrive
                if (
                    record == emsParticipation.Participant.LapRecords.Last()
                    && record.NextStarTime.HasValue
                )
                {
                    var nextEntry = new EmsStartlistEntry(emsParticipation)
                    {
                        Stage = emsParticipation.Participant.LapRecords.Count + 1,
                        StartTime = record.NextStarTime.Value,
                    };
                    if (startlists.ContainsKey(nextEntry.Stage))
                    {
                        startlists[nextEntry.Stage].Add(nextEntry);
                    }
                    else
                    {
                        startlists.Add(nextEntry.Stage, new EmsStartlist([nextEntry]));
                    }
                }
                toSkip++;
            }
        }
        return startlists;
    }

    public async Task<EmsParticipantsPayload> SafeSendParticipants()
    {
        var participants = await _participations
            .ReadAll(x => !x.IsEliminated() && !x.IsComplete()).Select(ParticipantEntryFactory.Create);
        var enduranceEvent = await _events.Read(0);
        return new EmsParticipantsPayload
        {
            Participants = participants.ToList(),
            EventId = enduranceEvent?.Id ?? 0,
        };
    }
}
