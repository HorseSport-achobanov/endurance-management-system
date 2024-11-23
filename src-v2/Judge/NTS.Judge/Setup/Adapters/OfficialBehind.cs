﻿using AngleSharp.Io;
using Not.Application.Behinds.Adapters;
using Not.Application.CRUD.Ports;
using NTS.Domain.Setup.Entities;
using NTS.Judge.Blazor.Setup.Officials;
using NTS.Judge.Core.Behinds;

namespace NTS.Judge.Setup.Adapters;

public class OfficialBehind : CrudBehind<Official, OfficialFormModel>
{
    public OfficialBehind(IRepository<Official> official, EventParentContext enduraceEventContext)
        : base(official, enduraceEventContext) { }

    protected override Official CreateEntity(OfficialFormModel model)
    {
        return Official.Create(model.Name, model.Role);
    }

    protected override Official UpdateEntity(OfficialFormModel model)
    {
        return Official.Update(model.Id, model.Name, model.Role);
    }
}
