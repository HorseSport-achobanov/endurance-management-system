﻿using Newtonsoft.Json;
using Not.Domain.Base;

namespace NTS.Domain.Core.Aggregates;

public class Official : AggregateRoot, IAggregateRoot
{
    [JsonConstructor]
    Official(int id, Person? person, OfficialRole? role)
        : base(id)
    {
        Person = Required(nameof(Person), person);
        Role = Required(nameof(Role), role);
    }

    public Official(Person? person, OfficialRole? role)
        : this(GenerateId(), person, role) { }

    public Person Person { get; }
    public OfficialRole Role { get; }

    public override string ToString()
    {
        return $"{Role}: {Person}";
    }
}
