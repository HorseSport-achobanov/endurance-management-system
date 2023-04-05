﻿using EnduranceJudge.Domain.Core.Models;
using EnduranceJudge.Domain.State.Competitions;
using EnduranceJudge.Domain.State.Participations;
using EnduranceJudge.Domain.Core.Exceptions;
using EnduranceJudge.Domain.State.Laps;
using EnduranceJudge.Domain.State.LapRecords;
using EnduranceJudge.Domain.State.Participants;
using System;
using System.Linq;
using static EnduranceJudge.Localization.Strings;

namespace EnduranceJudge.Domain.AggregateRoots.Manager.Aggregates;

public class ParticipationsAggregate : IAggregate
{
    private readonly Competition competitionConstraint;
    private readonly Participation participation;

    internal ParticipationsAggregate(Participation participation)
    {
        this.Number = participation.Participant.Number;
        var disqualifiedResult = participation.Participant.LapRecords.FirstOrDefault(
            rec => rec.Result?.IsNotQualified ?? false);
        this.IsDisqualified = disqualifiedResult != null;
        this.DisqualifiedCode = disqualifiedResult?.Result.Code;
        this.participation = participation;
        this.competitionConstraint = participation.CompetitionConstraint;
    }

    public string Number { get; }
    public bool IsDisqualified { get; }
    public string DisqualifiedCode { get; }
    public LapRecord CurrentLap => this.participation.Participant.LapRecords.Last();

    internal void Start()
    {
        this.CreateRecord(this.competitionConstraint.StartTime);
    }
    internal void Update(DateTime time)
    {
        if (this.IsDisqualified)
        {
            throw Helper.Create<ParticipantException>(PARTICIPATION_IS_DISQUALIFIED, this.Number);
        }
        var record = this.CurrentLap.Aggregate();
        var continueSequence = record.Update(time);
        if (continueSequence)
        {
            this.CreateRecord(this.CurrentLap.NextStarTime!.Value, time);
        }
    }
    
    internal void Add(Competition competition)
    {
        if (this.participation.CompetitionsIds.Any())
        {
            if (this.participation.CompetitionConstraint.Laps.Count != competition.Laps.Count)
            {
                throw Helper.Create<ParticipantException>(
                    CANNOT_ADD_PARTICIPATION_DIFFERENT_PHASE_COUNT_MESSAGE,
                    competition.Name);
            }
            for (var lapIndex = 0; lapIndex < this.participation.CompetitionConstraint.Laps.Count; lapIndex++)
            {
                var lapConstraint = this.participation.CompetitionConstraint.Laps[lapIndex];
                var lap = competition.Laps[lapIndex];
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (lapConstraint.LengthInKm != lap.LengthInKm)
                {
                    throw Helper.Create<ParticipantException>(
                        CANNOT_ADD_PARTICIPATION_DIFFERENT_PHASE_LENGTHS_MESSAGE,
                        competition.Name,
                        this.participation.CompetitionConstraint.Name,
                        lapIndex + 1,
                        lap.LengthInKm,
                        lapConstraint.LengthInKm);
                }
            }
        }
        this.participation.Add(competition.Id);
    }

    private LapRecord CreateRecord(DateTime startTime, DateTime? arriveTime = null)
    {
        if (this.NextLap == null)
        {
            throw Helper.Create<ParticipationException>(PARTICIPATION_HAS_ENDED_MESSAGE);
        }
        // startTime = FixDateForToday(startTime);
        var record = new LapRecord(startTime, this.NextLap);
        if (arriveTime.HasValue)
        {
            // arriveTime = FixDateForToday(arriveTime.Value);
            record.Aggregate().Arrive(arriveTime.Value);
        }
        this.participation.Participant.Add(record);
        return record;
    }

    private Lap NextLap
        => this.competitionConstraint.Laps.Count > this.participation.Participant.LapRecords.Count
            ? this.competitionConstraint.Laps[this.participation.Participant.LapRecords.Count]
            : null;

    // TODO: Remove after testing lap
    private DateTime FixDateForToday(DateTime date)
    {
        var today = DateTime.Today;
        today = today.AddHours(date.Hour);
        today = today.AddMinutes(date.Minute);
        today = today.AddSeconds(date.Second);
        today = today.AddMilliseconds(date.Millisecond);
        return today;
    }
}

public static partial class AggregateExtensions
{
    public static ParticipationsAggregate Aggregate(this Participation participation)
    {
        return new ParticipationsAggregate(participation);
    }
}
