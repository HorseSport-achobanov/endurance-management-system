﻿using Not.Application.CRUD.Ports;
using Not.Domain.Base;
using Not.Exceptions;

namespace Not.Application.Behinds;

public abstract class BehindContext<T>
    where T : AggregateRoot
{
    protected BehindContext(IRepository<T> repository)
    {
        Repository = repository;
    }

    protected IRepository<T> Repository { get; }
    public T? Entity { get; set; }

    public bool HasLoaded()
    {
        return Entity != null;
    }

    public async Task Persist()
    {
        GuardHelper.ThrowIfDefault(Entity);
        await Repository.Update(Entity);
    }
}
