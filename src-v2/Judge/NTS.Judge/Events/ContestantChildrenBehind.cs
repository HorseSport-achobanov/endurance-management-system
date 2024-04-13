﻿using Not.Blazor.Ports.Behinds;
using NTS.Domain.Setup.Entities;

namespace NTS.Judge.Events;

public class ContestantChildrenBehind : INotBehindWithChildren<Contestant>
{
    public Task Initialize(int id)
    {
        return Task.CompletedTask;
    }
}
