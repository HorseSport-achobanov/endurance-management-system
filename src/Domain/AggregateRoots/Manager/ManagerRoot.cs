﻿using EnduranceJudge.Core.Utilities;
using EnduranceJudge.Domain.AggregateRoots.Manager.Aggregates;
using EnduranceJudge.Domain.AggregateRoots.Manager.Aggregates.Startlists;
using EnduranceJudge.Domain.Core.Exceptions;
using EnduranceJudge.Domain.Core.Models;
using EnduranceJudge.Domain.State;
using EnduranceJudge.Domain.State.Competitions;
using EnduranceJudge.Domain.State.Participants;
using EnduranceJudge.Domain.State.Participations;
using EnduranceJudge.Domain.State.Performances;
using EnduranceJudge.Domain.State.TimeRecords;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using static EnduranceJudge.Localization.Strings;

namespace EnduranceJudge.Domain.AggregateRoots.Manager;

public class ManagerRoot : IAggregateRoot
{
    private readonly IState state;

    public ManagerRoot()
    {
        this.state = StaticProvider.GetService<IState>();
        // TODO: think this is no longer necessary.
        // Check is necessary due to Prism's initialization logic which uses reflection
        // to generate instances of views as part of the startup process.
        // These views are not used in the actual views during the application use cycle
        if (this.state?.Event == null)
        {
            return;
        }
    }

    public bool HasStarted()
        => this.state.Participants.Any(x => x.Participation.Performances.Any());

    public void Start()
    {
        this.ValidateConfiguration();
        var participants = this.state
            .Participants
            .Select(x => new ParticipantsAggregate(x.Participation))
            .ToList();
        foreach (var participant in participants)
        {
            participant.Start();
        }
        this.state.Event.HasStarted = true;
    }

    public void UpdatePerformance(int number, DateTime time)
    {
        var participant = this.GetParticipation(number);
        participant.UpdatePerformance(time);
    }

    public void CompletePerformance(int number, string code)
    {
        var participant = this.GetParticipation(number);
        var performance = participant.GetCurrent();
        performance.Complete(code);
    }

    public void ReInspection(int number, bool isRequired)
    {
        var participant = this.GetParticipation(number);
        var performance = participant.GetCurrent();
        if (performance == null)
        {
            throw Helper.Create<ParticipantException>(PARTICIPANT_HAS_NO_ACTIVE_PERFORMANCE_MESSAGE, number);
        }
        performance!.ReInspection(isRequired);
    }

    public void RequireInspection(int number, bool isRequired)
    {
        var participant = this.GetParticipation(number);
        var performance = participant.GetCurrent();
        if (performance == null)
        {
            throw Helper.Create<ParticipationException>(PARTICIPANT_HAS_NO_ACTIVE_PERFORMANCE_MESSAGE, number);
        }
        performance!.RequireInspection(isRequired);
    }

    public Performance EditRecord(ITimeRecordState state)
    {
        var record = this.state
            .Participants
            .SelectMany(part => part.TimeRecords)
            .FirstOrDefault(perf => perf.Equals(state));
        var manager = new PerformancesAggregate(record);
        manager.Edit(state);
        var performance = this.GetPerformance(state.Id);
        return performance;
    }

    public IEnumerable<StartModel> GetStartList(bool includePast)
    {
        var startList = new Startlist(this.state.Participants, includePast);
        return startList;
    }

    private ParticipantsAggregate GetParticipation(int number)
    {
        var participation = this.state
            .Participants
            .Where(x => x.Number == number)
            .Select(x => x.Participation)
            .FirstOrDefault();
        if (participation == null)
        {
            throw Helper.Create<ParticipantException>(PARTICIPANT_NUMBER_NOT_FOUND_MESSAGE, number);
        }
        var aggregate = new ParticipantsAggregate(participation);
        return aggregate;
    }

    private Performance GetPerformance(int id)
    {
        foreach (var participant in this.state.Participants)
        {
            foreach (var timeRecord in participant.TimeRecords)
            {
                var competition = participant.Participation.CompetitionConstraint;
                var index = participant.TimeRecords.ToList().IndexOf(timeRecord);
                var performance = new Performance(participant, competition.Phases, index);
                return performance;
            }
        }
        throw Helper.Create<TimeRecordException>(NOT_FOUND_BY_ID_MESSAGE, id);
    }

    private void ValidateConfiguration()
    {
        foreach (var competition in this.state.Event.Competitions)
        {
            if (competition.Phases.All(x => !x.IsFinal))
            {
                throw Helper.Create<CompetitionException>(
                    INVALID_COMPETITION_NO_FINAL_PHASE_MESSAGE,
                    competition.Name);
            }
        }
        foreach (var participant in this.state.Participants)
        {
            if (!participant.Participation.CompetitionsIds.Any())
            {
                throw Helper.Create<ParticipantException>(
                    INVALID_PARTICIPANT_NO_PARTICIPATIONS_MESSAGE,
                    participant.Number);
            }
            if (participant.Athlete.Country == null)
            {
                throw Helper.Create<ParticipantException>(INVALID_PARTICIPANT_NO_COUNTRY_MESSAGE, participant.Number);
            }
        }
    }
}
