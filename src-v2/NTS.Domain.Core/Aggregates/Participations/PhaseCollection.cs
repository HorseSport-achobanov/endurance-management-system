﻿using Not.Localization;
using System.Collections.ObjectModel;

namespace NTS.Domain.Core.Aggregates.Participations;

public class PhaseCollection : ReadOnlyCollection<Phase>
{
    public PhaseCollection(IEnumerable<Phase> phases) : base(phases.ToList())
    {
        var gate = 0d;
        for (var i = 0; i < this.Count; i++)
        {
            var phase = this[i];
            var distanceFromStart = gate += phase.Length;
            phase.InternalGate = $"{i + 1}/{distanceFromStart:0.##}";
        }
        Current = this.FirstOrDefault(x => !x.IsComplete);
    }

    internal Phase? Current { get; private set; } 
    internal int CurrentNumber => this.NumberOf(Current ?? this.Last());
    public double Distance => this.Sum(x => x.Length);
    internal Timestamp? OutTime => this.LastOrDefault(x => x.OutTime != null)?.OutTime;

    public override string ToString()
    {
        return $"{Distance}{"km".Localize()}: {this.Count(x => x.IsComplete)}/{this.Count}";
    }

    internal void Next()
    {
        if (Current == null)
        {
            return;
        }
        var currentIndex = this.IndexOf(Current);
        if (Count <= currentIndex)
        {
            return;
        }
        Current = this[currentIndex + 1];
    }
}
