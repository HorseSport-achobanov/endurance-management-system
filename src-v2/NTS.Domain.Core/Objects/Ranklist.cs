﻿using System.Collections;
using NTS.Domain.Core.Configuration;
using NTS.Domain.Core.Entities;
using NTS.Domain.Core.Objects.Regional;

namespace NTS.Domain.Core.Objects;

public class Ranklist : IReadOnlyList<RankingEntry>
{
    static readonly FeiRanker _feiRanker = new();
    static readonly Ranker[] _regionalRankers = [new BulgariaRanker()];
    readonly Ranking _ranking;

    List<RankingEntry> _entries;

    public Ranklist(Ranking ranking)
    {
        _entries = Rank(ranking);
        _ranking = ranking;
    }

    #region IReadOnlyList implementation

    public int Count => _entries.Count;
    public RankingEntry this[int index] => _entries[index];

    public IEnumerator<RankingEntry> GetEnumerator()
    {
        return _entries.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    #endregion

    public int RankingId => _ranking.Id;
    public string Name => _ranking.Name;
    public AthleteCategory Category => _ranking.Category;
    public CompetitionRuleset Ruleset => _ranking.Ruleset;
    public string Title => $"{Category}: {Name}";

    public void Update(Participation participation)
    {
        var existing = _ranking.Entries.FirstOrDefault(x => x.Participation == participation);
        if (existing == null)
        {
            return;
        }
        existing.Participation = participation;
        _entries = Rank(_ranking);
    }

    static List<RankingEntry> Rank(Ranking ranking)
    {
        var ranker = StaticOptions.ShouldUseRegionalRanker(ranking.Ruleset)
            ? GetRanker(StaticOptions.RegionalConfiguration)
            : _feiRanker;
        var ranked = ranker.Rank(ranking);
        var rank = 0;
        foreach (var entry in ranked)
        {
            entry.Rank = ++rank;
        }
        return ranked;
    }

    static Ranker GetRanker(IRegionalConfiguration? configuration)
    {
        return _regionalRankers.FirstOrDefault(x =>
                x.CountryIsoCode == configuration?.CountryIsoCode
            ) ?? _feiRanker;
    }
}
