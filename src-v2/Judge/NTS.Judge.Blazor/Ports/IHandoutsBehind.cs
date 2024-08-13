﻿using Not.Blazor.Ports.Behinds;
using Not.Injection;
using Not.Startup;
using NTS.Domain.Core.Objects;

namespace NTS.Judge.Blazor.Ports;

public interface IHandoutsBehind : IStartupInitializer, IObservableBehind, ISingletonService
{
    IReadOnlyList<HandoutDocument> Handouts { get; }
    Task<IEnumerable<HandoutDocument>> PopAll();
}
