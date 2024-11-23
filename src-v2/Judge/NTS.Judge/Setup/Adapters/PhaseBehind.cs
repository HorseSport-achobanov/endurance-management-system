﻿using Not.Application.Behinds.Adapters;
using Not.Application.CRUD.Ports;
using NTS.Domain.Setup.Entities;
using NTS.Judge.Blazor.Setup.Phases;
using NTS.Judge.Core.Behinds;

namespace NTS.Judge.Setup.Adapters;

public class PhaseBehind : CrudBehind<Phase, PhaseFormModel>
{
    readonly CompetitionParentContext _parentContext;

    public PhaseBehind(IRepository<Phase> phase, CompetitionParentContext parentContext)
        : base(phase, parentContext)
    {
        _parentContext = parentContext;
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
