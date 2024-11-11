﻿using Not.Domain;
using Not.Injection;

namespace Not.Application.Ports.CRUD;

public interface ICreateChild<T> : ITransient
    where T : DomainEntity
{
    Task<T> Create(int parentId, T child);
}
