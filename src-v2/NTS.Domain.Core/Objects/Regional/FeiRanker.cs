﻿using NTS.Domain.Core.Entities;

namespace NTS.Domain.Core.Objects.Regional;

internal class FeiRanker : Ranker
{
    protected IOrderedEnumerable<RankingEntry> OrderByNotEliminatedAndRanked(
        IEnumerable<RankingEntry> entries
    )
    {
        return entries
            .OrderBy(x => x.Participation.IsEliminated())
            .ThenBy(x => x.IsNotRanked)
            .ThenBy(x => !x.Participation.IsComplete());
    }

    public override List<RankingEntry> Rank(Ranking ranking)
    {
        return OrderByNotEliminatedAndRanked(ranking.Entries)
            .ThenBy(x => x.Participation.Phases.Last().ArriveTime)
            .ToList();
    }
}

internal abstract class Ranker
{
    public abstract List<RankingEntry> Rank(Ranking ranking);

    public string? CountryIsoCode { get; protected set; }
}
