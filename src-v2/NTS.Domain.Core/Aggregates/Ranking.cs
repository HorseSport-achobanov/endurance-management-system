﻿using System.Collections.ObjectModel;
using Newtonsoft.Json;
using Not.Domain.Base;
using NTS.Domain.Core.Aggregates.Participations;

namespace NTS.Domain.Core.Aggregates;

public class Ranking : AggregateRoot, IAggregateRoot
{
    [JsonConstructor]
    Ranking(
        int id,
        string name,
        CompetitionRuleset ruleset,
        AthleteCategory category,
        ReadOnlyCollection<RankingEntry> entries
    )
        : base(id)
    {
        Name = name;
        Ruleset = ruleset;
        Category = category;
        Entries = entries;
    }

    public Ranking(
        Competition competition,
        AthleteCategory category,
        IEnumerable<RankingEntry> entries
    )
        : this(GenerateId(), competition.Name, competition.Ruleset, category, new(entries.ToList()))
    { }

    public string Name { get; }
    public CompetitionRuleset Ruleset { get; }
    public AthleteCategory Category { get; }
    public ReadOnlyCollection<RankingEntry> Entries { get; }

    public override string ToString()
    {
        return $"{Name} {Category}: {Entries.Count}";
    }
}
