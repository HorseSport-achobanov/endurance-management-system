﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Domain.AggregateRoots.Manager.Aggregates.Startlists;
using Core.Enums;

namespace Core.Application.Rpc.Procedures;

public interface IStartlistClientProcedures
{
    Task ReceiveEntry(StartlistEntry entry, CollectionAction action);
}

public interface IStartlistHubProcedures
{
    Dictionary<int, Startlist> SendStartlist();
}
