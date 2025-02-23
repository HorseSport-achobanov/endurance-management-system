﻿using Not.Storage.States;
using NTS.Domain.Setup.Aggregates;

namespace NTS.Storage.Setup;

public class SetupState
    : NState,
        ITreeState<EnduranceEvent>,
        ISetState<Loop>,
        ISetState<Horse>,
        ISetState<Athlete>,
        ISetState<Combination>,
        ISetState<Participation>,
        ISetState<Competition>,
        ISetState<Phase>,
        ISetState<Official>
{
    EnduranceEvent? ITreeState<EnduranceEvent>.Root
    {
        get => EnduranceEvent;
        set => EnduranceEvent = value;
    }
    List<Loop> ISetState<Loop>.EntitySet => Loops;
    List<Horse> ISetState<Horse>.EntitySet => Horses;
    List<Athlete> ISetState<Athlete>.EntitySet => Athletes;
    List<Combination> ISetState<Combination>.EntitySet => Combinations;
    List<Participation> ISetState<Participation>.EntitySet => Participations;
    List<Competition> ISetState<Competition>.EntitySet => Competitions;
    List<Official> ISetState<Official>.EntitySet => Officials;
    List<Phase> ISetState<Phase>.EntitySet => Phases;

    // The order here is very important due to how EntityReferenceEqualityGuardConverter works
    // Root level entities (Loop, Horse etc) MUST precede their parent entities (Combination, Phase)
    // in order to be the first to be serialized. Otherwise updates on those entities will be ignored
    // because the obsoleted entities will be serialized first and the updates will be lost to a $domainRef
    public List<Loop> Loops { get; } = [];
    public List<Official> Officials { get; } = [];
    public List<Horse> Horses { get; } = [];
    public List<Athlete> Athletes { get; } = [];
    public List<Phase> Phases { get; } = [];
    public List<Combination> Combinations { get; } = [];
    public List<Participation> Participations { get; } = [];
    public List<Competition> Competitions { get; } = [];
    public EnduranceEvent? EnduranceEvent { get; set; }
}
