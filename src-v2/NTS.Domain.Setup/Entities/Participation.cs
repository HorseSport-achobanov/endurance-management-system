﻿using Newtonsoft.Json;

namespace NTS.Domain.Setup.Entities;

public class Participation : DomainEntity, ISummarizable
{
    const double CHILDREN_MIN_SPEED = 8;
    const double CHILDREN_MAX_SPEED = 12;
    const double MIN_SPEED = 10;
    const double MAX_SPEED = 16;

    public static Participation Create(DateTimeOffset? newStart, bool isUnranked, Combination? combination, double? maxSpeedOverride)
        => new(newStart, isUnranked, combination, maxSpeedOverride);

    public static Participation Update(int id, DateTimeOffset? newStart, bool isUnranked, Combination? combination, double? maxSpeedOverride)
        => new(id, newStart, isUnranked, combination, maxSpeedOverride);
    
    [JsonConstructor]
    private Participation(int id, DateTimeOffset? startTimeOverride, bool isUnranked, Combination? combination, double? maxSpeedOverride)
        : base(id)
    {
        StartTimeOverride = startTimeOverride;
        IsNotRanked = isUnranked;
        Combination = Required(nameof(Combination), combination);
        MaxSpeedOverride = maxSpeedOverride;
    }

    private Participation(DateTimeOffset? startTimeOverride, bool isUnranked, Combination? combination, double? maxSpeedOverride) : this(
        GenerateId(),
        IsFutureTime(startTimeOverride),
        isUnranked,
        combination,
        maxSpeedOverride)
    {
    }

    static DateTimeOffset? IsFutureTime(DateTimeOffset? startTimeOverride)
    {
        if (startTimeOverride != null && startTimeOverride.Value <= DateTimeOffset.Now)
        {
            throw new DomainException(nameof(StartTimeOverride), "Please select future time");
        }
        return startTimeOverride;
    }

    public Combination Combination { get; }
    public bool IsNotRanked { get; }
    public DateTimeOffset? StartTimeOverride { get; }
    public double? MinAverageSpeed { get; private set; }
    public double? MaxAverageSpeed { get; private set; }
    public double? MaxSpeedOverride { get; private set; }

    internal void SetSpeedLimits(CompetitionType competitionType)
    {
        var athleteCategory = Combination.Athlete.Category;
        MinAverageSpeed = MIN_SPEED;
        if (competitionType == CompetitionType.Qualification)
        {
            if (athleteCategory == AthleteCategory.Children)
            {
                MinAverageSpeed = CHILDREN_MIN_SPEED;
                MaxAverageSpeed = CHILDREN_MAX_SPEED;
            }
            else
            {
                MaxAverageSpeed = MAX_SPEED;
            }
        }
        if (MaxSpeedOverride != null)
        {
            MaxAverageSpeed = MaxSpeedOverride;
        }
    }

    public string Summarize()
    {
        var summary = new Summarizer(this);
        return summary.ToString();
    }

    public override string ToString()
    {
        var startTimeMessage = StartTimeOverride != null 
            ? $"start: {StartTimeOverride.Value.ToLocalTime().TimeOfDay} "
            : null;
        var isUnrankedMessage = IsNotRanked
            ? "not-ranked"
            : null;
        return Combine(Combination, startTimeMessage, isUnrankedMessage);
    }
}
