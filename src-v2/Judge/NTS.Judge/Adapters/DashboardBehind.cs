﻿using Not.Application.Ports.CRUD;
using Not.Domain;
using NTS.Domain.Core.Entities;
using NTS.Judge.Blazor.Ports;

namespace NTS.Judge.Adapters;

public class DashboardBehind : IDashboardBehind
{
    private readonly IRepository<Domain.Setup.Entities.Event> _setupRepository;
    private readonly IRepository<Event> _coreEventRespository;
    private readonly IRepository<Official> _coreOfficialRepository;

    public DashboardBehind(
        IRepository<Domain.Setup.Entities.Event> setupRepository,
        IRepository<Event> coreEventRespository,
        IRepository<Official> coreOfficialRepository)
    {
        _setupRepository = setupRepository;
        _coreEventRespository = coreEventRespository;
        _coreOfficialRepository = coreOfficialRepository;
    }

    public async Task Initialize()
    {
        var setupEvent = await _setupRepository.Read(0);
        if (setupEvent == null)
        {
            // TODO: Create ValidationException containing localization logic and inherit form it in DomainException. Use that here instead
            throw new DomainException("Cannot start - event is not configured");
        }
        await CreateEvent(setupEvent);
        await CreateOfficials(setupEvent.Officials);
    }

    private async Task CreateEvent(Domain.Setup.Entities.Event setupEvent)
    {
        var @event = new Event(setupEvent.Country, setupEvent.Place, null, null, null);
        await _coreEventRespository.Create(@event);
    }

    private async Task CreateOfficials(IEnumerable<Domain.Setup.Entities.Official> setupOfficials)
    {
        foreach (var setupOfficial in setupOfficials)
        {
            var official = new Official(setupOfficial.Person, setupOfficial.Role);
            await _coreOfficialRepository.Create(official);
        }
    }
}
