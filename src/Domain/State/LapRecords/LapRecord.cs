﻿using EnduranceJudge.Domain.AggregateRoots.Manager.Aggregates;
using EnduranceJudge.Domain.Core.Models;
using EnduranceJudge.Domain.State.Laps;
using EnduranceJudge.Domain.State.Results;
using System;

namespace EnduranceJudge.Domain.State.LapRecords;

public class LapRecord : DomainBase<LapRecordException>, ILapRecordState
{
    public LapRecord() {}
    public LapRecord(DateTime startTime, Lap lap) : base(GENERATE_ID)
    {
        this.StartTime = startTime;
        this.Lap = lap;
    }

    public Lap Lap { get; private set; }
    public DateTime StartTime { get; set; } // TODO: set to private/internal after testing
    public DateTime? ArrivalTime { get; set; } // TODO: set to private/internal after testing
    public DateTime? InspectionTime { get; set; } // TODO: set to private/internal after testing
    public DateTime? ReInspectionTime { get; set; } // TODO: set to private/internal after testing
    public bool IsReInspectionRequired { get; internal set; }
    public bool IsRequiredInspectionRequired { get; internal set; }
    public Result Result { get; internal set; }

    public DateTime? VetGateTime
        => this.ReInspectionTime ?? this.InspectionTime;
    public DateTime? NextStarTime
        => this.VetGateTime?.AddMinutes(this.Lap.RestTimeInMins);
}

public static partial class AggregateExtensions
{
    public static LapRecordsAggregate Aggregate(this LapRecord record)
        => new (record);
}
