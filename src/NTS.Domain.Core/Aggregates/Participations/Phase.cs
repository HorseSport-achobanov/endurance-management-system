﻿using Newtonsoft.Json;
using Not.Domain.Base;
using Not.Domain.Exceptions;
using Not.Localization;
using NTS.Domain.Core.StaticOptions;
using static NTS.Domain.Core.Aggregates.SnapshotResultType;

namespace NTS.Domain.Core.Aggregates.Participations;

public class Phase : AggregateRoot
{
    public static Phase ImportFromEMS(
        double length,
        int maxRecovery,
        int rest,
        CompetitionRuleset competitionType,
        bool isFinal,
        TimeSpan? compulsoryThreshold,
        Timestamp startTimestamp,
        DateTime? arriveTime,
        DateTime? inspectTime,
        DateTime? reinspectTime,
        bool isReinspectionRequested,
        bool isRequiredInspectionRequested,
        bool isCompulsoryRequiredInspectionRequested
    )
    {
        var phase = new Phase(
            length,
            maxRecovery,
            rest,
            competitionType,
            isFinal,
            compulsoryThreshold,
            startTimestamp.ToDateTimeOffset()
        )
        {
            StartTime = startTimestamp,
        };
        if (arriveTime != null)
        {
            phase.ArriveTime = new Timestamp(arriveTime.Value);
        }
        if (inspectTime != null)
        {
            phase.PresentTime = new Timestamp(inspectTime.Value);
        }
        if (reinspectTime != null)
        {
            phase.RepresentTime = new Timestamp(reinspectTime.Value);
        }
        phase.IsReinspectionRequested = isReinspectionRequested;
        phase.IsRequiredInspectionRequested = isRequiredInspectionRequested;
        phase.IsRequiredInspectionCompulsory = isCompulsoryRequiredInspectionRequested;

        return phase;
    }

    // TODO: settings - Add setting for separate final. This is useful for some events such as Shumen where we need separate detection for the actual final
    bool _isSeparateFinish = false;

    [JsonConstructor]
    Phase(
        int id,
        string gate,
        double length,
        int maxRecovery,
        int? rest,
        CompetitionRuleset ruleset,
        bool isFinal,
        TimeSpan? compulsoryThresholdSpan,
        Timestamp? startTime,
        Timestamp? arriveTime,
        Timestamp? presentTime,
        Timestamp? representTime,
        bool isRepresentationRequested,
        bool isRequiredInspectionRequested,
        bool isRequiredInspectionCompulsory
    )
        : base(id)
    {
        Gate = gate;
        Length = length;
        MaxRecovery = maxRecovery;
        Rest = rest;
        Ruleset = ruleset;
        IsFinal = isFinal;
        StartTime = startTime;
        ArriveTime = arriveTime;
        PresentTime = presentTime;
        RepresentTime = representTime;
        IsReinspectionRequested = isRepresentationRequested;
        IsRequiredInspectionRequested = isRequiredInspectionRequested;
        IsRequiredInspectionCompulsory = isRequiredInspectionCompulsory;
        CompulsoryThresholdSpan = compulsoryThresholdSpan;
    }

    public Phase(
        double length,
        int maxRecovery,
        int? rest,
        CompetitionRuleset competitionRuleset,
        bool isFinal,
        TimeSpan? compulsoryThresholdSpan,
        DateTimeOffset? startTime
    )
        : this(
            GenerateId(),
            "",
            length,
            maxRecovery,
            rest,
            competitionRuleset,
            isFinal,
            compulsoryThresholdSpan,
            Timestamp.Create(startTime),
            null,
            null,
            null,
            false,
            false,
            false
        ) { }

    Timestamp? VetTime => RepresentTime ?? PresentTime;

    public string Gate { get; private set; }
    public double Length { get; }
    public int MaxRecovery { get; }
    public int? Rest { get; }
    public CompetitionRuleset Ruleset { get; }
    public bool IsFinal { get; }
    public Timestamp? StartTime { get; internal set; }
    public Timestamp? ArriveTime { get; private set; }
    public Timestamp? PresentTime { get; private set; }
    public Timestamp? RepresentTime { get; private set; }
    public bool IsReinspectionRequested { get; internal set; }
    public bool IsRequiredInspectionRequested { get; internal set; }
    public bool IsRequiredInspectionCompulsory { get; private set; }
    public TimeSpan? CompulsoryThresholdSpan { get; private set; }

    internal SnapshotResult Process(Snapshot snapshot)
    {
        return snapshot.Type switch
        {
            SnapshotType.Vet => Inspect(snapshot),
            SnapshotType.Stage => Arrive(snapshot),
            SnapshotType.Final => Finish(snapshot),
            SnapshotType.Automatic => Automatic(snapshot),
            _ => guardUnknownSnapshot(snapshot),
        };
        static SnapshotResult guardUnknownSnapshot(Snapshot snapshot)
        {
            var message = $"Invalid snapshot '{snapshot.GetType()}'";
            throw GuardHelper.Exception(message);
        }
    }

    internal void Update(IPhaseState state)
    {
        if (state.StartTime != null)
        {
            if (state.ArriveTime < state.StartTime)
            {
                throw new DomainException(
                    nameof(ArriveTime),
                    "Arrive Time cannot be sooner than Start Time"
                );
            }
            if (state.PresentTime < state.StartTime)
            {
                throw new DomainException(
                    nameof(PresentTime),
                    "Inspect Time cannot be sooner than Start Time"
                );
            }
            if (state.RepresentTime < state.ArriveTime)
            {
                throw new DomainException(
                    nameof(RepresentTime),
                    "Reinspect Time cannot be sooner than Start Time"
                );
            }
        }
        StartTime = Timestamp.Create(state.StartTime);
        ArriveTime = Timestamp.Create(state.ArriveTime);
        PresentTime = Timestamp.Create(state.PresentTime);
        RepresentTime = Timestamp.Create(state.RepresentTime);
    }

    internal bool ViolatesRecoveryTime()
    {
        return GetRecoverySpan() > TimeSpan.FromMinutes(MaxRecovery);
    }

    internal bool ViolatesSpeedRestriction(Speed? minSpeed, Speed? maxSpeed)
    {
        var averageSpeed = GetAverageSpeed();
        return averageSpeed < minSpeed || averageSpeed > maxSpeed;
    }

    internal void RequestRequiredInspection()
    {
        if (IsRequiredInspectionRequested)
        {
            return;
        }
        if (IsRequiredInspectionCompulsory)
        {
            throw new DomainException("Required inspection is compulsory");
        }
        IsRequiredInspectionRequested = true;
    }

    internal void DisableReinspection()
    {
        if (!IsReinspectionRequested)
        {
            return;
        }
        if (RepresentTime != null)
        {
            throw new DomainException(
                "Cannot disable Reinspection because time of Reinspection is already present"
            );
        }
        IsReinspectionRequested = false;
    }

    internal void SetGate(int number, double totalDistanceSoFar)
    {
        Gate = $"GATE{number}/{totalDistanceSoFar:0.##}";
    }

    public override string ToString()
    {
        var arrive = $"{LocalizationHelper.Get("ARR")}:{ArriveTime}";
        var present = $"{LocalizationHelper.Get("IN")}:{PresentTime}";
        var complete = IsComplete() ? "complete" : "";
        return Combine(Gate, arrive, present, complete);
    }

    public Timestamp? GetRequiredInspectionTime()
    {
        if (Rest == null)
        {
            return null;
        }
        var span = TimeSpan.FromMinutes(Rest.Value - 15); //TODO: settings
        return VetTime?.Add(span);
    }

    public Timestamp? GetOutTime()
    {
        if (ArriveTime == null || Rest == null)
        {
            return null;
        }
        var span = TimeSpan.FromMinutes(Rest.Value);
        return VetTime?.Add(span);
    }

    public TimeInterval? GetLoopSpan()
    {
        return ArriveTime - StartTime;
    }

    public TimeInterval? GetPhaseSpan()
    {
        return IsFinal ? GetLoopSpan() : VetTime - StartTime;
    }

    public TimeInterval? GetRecoverySpan()
    {
        return VetTime - ArriveTime;
    }

    public Speed? GetAverageLoopSpeed()
    {
        return Length / GetLoopSpan();
    }

    public Speed? GetAveragePhaseSpeed()
    {
        return Length / GetPhaseSpan();
    }

    public Speed? GetAverageSpeed()
    {
        if (StaticOption.ShouldOnlyUseAverageLoopSpeed(Ruleset))
        {
            return GetAverageLoopSpeed();
        }
        return IsFinal ? GetAverageLoopSpeed() : GetAveragePhaseSpeed();
    }

    public bool IsComplete()
    {
        if (IsReinspectionRequested && RepresentTime == null)
        {
            return false;
        }
        if (ArriveTime == null || PresentTime == null)
        {
            return false;
        }
        return true;
    }

    SnapshotResult Automatic(Snapshot snapshot)
    {
        if (ArriveTime == null && IsFinal)
        {
            return Finish(snapshot);
        }
        if (ArriveTime == null)
        {
            return Arrive(snapshot);
        }
        if (PresentTime == null || RepresentTime == null)
        {
            return Inspect(snapshot);
        }
        return SnapshotResult.NotApplied(snapshot, NotAppliedDueToInapplicableAutomatic);
    }

    SnapshotResult Finish(Snapshot snapshot)
    {
        if (_isSeparateFinish && !IsFinal)
        {
            return SnapshotResult.NotApplied(snapshot, NotAppliedDueToSeparateStageLine);
        }
        if (ArriveTime != null)
        {
            return SnapshotResult.NotApplied(snapshot, NotAppliedDueToDuplicateArrive);
        }

        ArriveTime = snapshot.Timestamp;
        CheckCompulsoryThreshold();
        return SnapshotResult.Applied(snapshot);
    }

    SnapshotResult Arrive(Snapshot snapshot)
    {
        if (_isSeparateFinish && IsFinal)
        {
            return SnapshotResult.NotApplied(snapshot, NotAppliedDueToSeparateFinishLine);
        }
        if (ArriveTime != null)
        {
            return SnapshotResult.NotApplied(snapshot, NotAppliedDueToDuplicateArrive);
        }

        ArriveTime = snapshot.Timestamp;
        CheckCompulsoryThreshold();
        return SnapshotResult.Applied(snapshot);
    }

    SnapshotResult Inspect(Snapshot snapshot)
    {
        if (IsReinspectionRequested && RepresentTime != null && PresentTime != null)
        {
            return SnapshotResult.NotApplied(snapshot, NotAppliedDueToDuplicateInspect);
        }
        if (snapshot.Timestamp <= ArriveTime)
        {
            throw new DomainException($"Presentation time cannot be before arrival '{ArriveTime}'");
        }

        if (IsReinspectionRequested)
        {
            if (snapshot.Timestamp <= PresentTime)
            {
                throw new DomainException(
                    $"Representation time cannot be before presentation '{PresentTime}'"
                );
            }
            RepresentTime = snapshot.Timestamp;
        }
        else
        {
            PresentTime = snapshot.Timestamp;
        }
        CheckCompulsoryThreshold();
        return SnapshotResult.Applied(snapshot);
    }

    void CheckCompulsoryThreshold()
    {
        if (CompulsoryThresholdSpan == null || IsFinal)
        {
            return;
        }
        IsRequiredInspectionCompulsory = GetRecoverySpan() >= CompulsoryThresholdSpan;
    }
}

public interface IPhaseState
{
    int Id { get; }
    public DateTimeOffset? StartTime { get; }
    public DateTimeOffset? ArriveTime { get; }
    public DateTimeOffset? PresentTime { get; }
    public DateTimeOffset? RepresentTime { get; }
}
