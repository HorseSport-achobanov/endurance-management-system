﻿using Not.Application.Behinds.Adapters;
using Not.Application.CRUD.Ports;
using NTS.Domain.Core.Aggregates;
using NTS.Domain.Core.Objects;
using NTS.Judge.Blazor.Core.Startlists.History;
using NTS.Judge.Blazor.Core.Startlists.Upcoming;

namespace NTS.Judge.Core.Behinds.Adapters;

public class StartlistBehind : ObservableBehind, IStartlistUpcoming, IStartlistHistory
{
    readonly IRepository<Participation> _participationRepository;
    StartList? _startlist;

    public StartlistBehind(IRepository<Participation> participationRepository)
    {
        _participationRepository = participationRepository;
    }

    public IReadOnlyList<Start> Upcoming => _startlist?.Upcoming ?? [];
    public IReadOnlyList<Start> History => _startlist?.History ?? [];

    protected override async Task<bool> PerformInitialization(params IEnumerable<object> arguments)
    {
        var participations = await _participationRepository.ReadAll();
        _startlist = new StartList(participations, EmitChange);
        ;
        return _startlist.Any();
    }
}
