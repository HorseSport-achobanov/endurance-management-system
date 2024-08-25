﻿using static NTS.Domain.Core.Aggregates.Participations.SnapshotResultType;

namespace NTS.Domain.Core.Aggregates.Participations;

public class Phase : DomainEntity, IPhaseState
{
    // TODO: settings - Add setting for separate final. This is useful for some events such as Shumen where we need separate detection for the actual final
    bool _isSeparateFinish = false;

    internal string InternalGate { get; set; }
    private Timestamp? VetTime => ReinspectTime ?? InspectTime;
    private bool IsFeiRulesAndNotFinal => CompetitionType == CompetitionType.FEI && !IsFinal;

    public Phase(double length, int maxRecovery, int rest, CompetitionType competitionType, bool isFinal, int? criRecovery)
    {
        Length = length;
        MaxRecovery = maxRecovery;
        Rest = rest;
        CompetitionType = competitionType;
        IsFinal = isFinal;
        CRIRecovery = criRecovery;
    }

    public string Gate => $"GATE{InternalGate}"; // TODO: fix InternalGate complexity
    public double Length { get; private set; }
    public int MaxRecovery { get; private set; }
    public int Rest { get; private set; }
    public CompetitionType CompetitionType { get; private set; }
    public bool IsFinal { get; private set; }
    public int? CRIRecovery { get; private set; } // TODO: int CRIRecovery? wtf?
    
    //> Temporarily set to public for EMS import testing
    public Timestamp? StartTime { get; set; }
    public Timestamp? ArriveTime { get; set; }
    public Timestamp? InspectTime { get; set; }
    public bool IsReinspectionRequested { get; set; }

    public Timestamp? ReinspectTime { get; set; }
    public bool IsRIRequested { get; set; }
    public bool IsCRIRequested { get; set; }
    //< Temporarily set to public for EMS import testing

    public Timestamp? RequiredInspectionTime => VetTime?.Add(TimeSpan.FromMinutes(Rest - 15)); //TODO: settings?
    public Timestamp? OutTime => ArriveTime == null ? null : VetTime?.Add(TimeSpan.FromMinutes(Rest));
    public TimeInterval? LoopSpan => ArriveTime - StartTime;
    public TimeInterval? PhaseSpan => VetTime - StartTime;
    public TimeInterval? Span => IsFeiRulesAndNotFinal ? PhaseSpan : LoopSpan;
    public TimeInterval? RecoverySpan => VetTime - ArriveTime;
    public Speed? AverageLoopSpeed => Length / LoopSpan;
    public Speed? AveragePhaseSpeed => Length / PhaseSpan;
    public Speed? AverageSpeed => IsFeiRulesAndNotFinal ? AveragePhaseSpeed : AverageLoopSpeed;
    public bool IsComplete()
    {
        if (IsReinspectionRequested && ReinspectTime == null)
        {
            return false;
        }
        if (ArriveTime == null || InspectTime == null)
        {
            return false;
        }
        return true;
    }

    internal SnapshotResult Process(Snapshot snapshot)
    {
        if (snapshot is FinishSnapshot finishSnapshot)
        {
            return Finish(finishSnapshot);
        }
        else if (snapshot is StageSnapshot stageSnapshot)
        {
            return Arrive(stageSnapshot);
        }
        else if (snapshot is VetgateSnapshot vetgateSnapshot)
        {
            return Inspect(vetgateSnapshot);
        }
        else
        {
            throw GuardHelper.Exception($"Invalid snapshot '{snapshot.GetType()}'");
        }
    }

    internal void Update(IPhaseState state)
    {
        if (state.StartTime != null)
        {
            if (state.ArriveTime < state.StartTime)
            {
                throw new DomainException(nameof(ArriveTime), "Arrive Time cannot be sooner than Start Time");
            }
            if (state.InspectTime < state.StartTime)
            {
                throw new DomainException(nameof(InspectTime), "Inspect Time cannot be sooner than Start Time");
            }
            if (state.ReinspectTime < state.ArriveTime)
            {
                throw new DomainException(nameof(ReinspectTime), "Reinspect Time cannot be sooner than Start Time");
            }
        }
        StartTime = state.StartTime;
        ArriveTime = state.ArriveTime;
        InspectTime = state.InspectTime;
        ReinspectTime = state.ReinspectTime;
    }

    internal bool ViolatesRecoveryTime()
    {
        return RecoverySpan > TimeSpan.FromMinutes(MaxRecovery);
    }

    internal bool ViolatesSpeedRestriction(Speed? minSpeed, Speed? maxSpeed)
    {
        return AverageSpeed < minSpeed || AverageSpeed > maxSpeed;
    }

    SnapshotResult Finish(FinishSnapshot snapshot)
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
        HandleCRI();
        return SnapshotResult.Applied(snapshot);
    }

    SnapshotResult Arrive(StageSnapshot snapshot)
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
        HandleCRI();
        return SnapshotResult.Applied(snapshot);
    }

    SnapshotResult Inspect(Snapshot snapshot)
    {
        if (IsReinspectionRequested && ReinspectTime != null && InspectTime != null)
        {
            return SnapshotResult.NotApplied(snapshot, NotAppliedDueToDuplicateInspect);
        }

        if (IsReinspectionRequested)
        {
            ReinspectTime = snapshot.Timestamp;
        }
        else
        {
            InspectTime = snapshot.Timestamp;
        }
        HandleCRI();
        return SnapshotResult.Applied(snapshot);
    }

    private void HandleCRI()
    {
        if (CRIRecovery == null)
        {
            return;
        }
        IsCRIRequested = RecoverySpan >= TimeSpan.FromMinutes(CRIRecovery.Value);
    }
}

public interface IPhaseState
{
    int Id { get; }
    public Timestamp? StartTime { get; }
    public Timestamp? ArriveTime { get; }
    public Timestamp? InspectTime { get; }
    public Timestamp? ReinspectTime { get; }
}

public enum PhaseState
{
    Ongoing = 1,
    Arrived = 2,
    Presented = 3
}
