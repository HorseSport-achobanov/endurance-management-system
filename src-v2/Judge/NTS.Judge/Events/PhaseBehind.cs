﻿using Not.Application.Adapters.Behinds;
using Not.Application.Ports.CRUD;
using Not.Exceptions;
using NTS.Domain.Setup.Entities;
using NTS.Judge.Blazor.Pages.Setup.Phases;

namespace NTS.Judge.Events;

public class PhaseBehind : SimpleCrudBehind<Phase, PhaseFormModel>
{
    private readonly IRepository<Competition> _competitions;

    public PhaseBehind(IRepository<Competition> competitions, IRepository<Phase> phase) : base(phase)
    {
        _competitions = competitions;
    }

    protected override async Task<bool> PerformInitialization(params IEnumerable<object> arguments)
    {
        if (arguments.FirstOrDefault() is not int competitionId)
        {
            throw GuardHelper.Exception("Competition ID is required");
        }
        if (CompetitionChildrenBehind.Competition?.Id == competitionId)
        {
            return false;
        }
        
        var competition = await _competitions.Read(competitionId);
        GuardHelper.ThrowIfDefault(competition);

        CompetitionChildrenBehind.Competition = competition;
        ObservableCollection.AddRange(competition.Phases);

        return false; // This allows us to change the parent competition
    }

    protected override async Task OnBeforeCreate(Phase entity)
    {
        CompetitionChildrenBehind.Competition!.Add(entity);
        await _competitions.Update(CompetitionChildrenBehind.Competition);
    }

    protected override async Task OnBeforeUpdate(Phase entity)
    {
        CompetitionChildrenBehind.Competition!.Update(entity);
        await _competitions.Update(CompetitionChildrenBehind.Competition);
    }

    protected override async Task OnBeforeDelete(Phase entity)
    {
        CompetitionChildrenBehind.Competition!.Remove(entity);
        await _competitions.Update(CompetitionChildrenBehind.Competition);
    }

    protected override Phase CreateEntity(PhaseFormModel model)
    {
        return Phase.Create(model.Loop, model.Recovery, model.Rest);
    }

    protected override Phase UpdateEntity(PhaseFormModel model)
    {
        return Phase.Update(model.Id, model.Loop, model.Recovery, model.Rest);
    }
}
