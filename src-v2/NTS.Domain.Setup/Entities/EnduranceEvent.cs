﻿using Newtonsoft.Json;
using Not.Domain.Base;
using Not.Domain.Exceptions;
using NTS.Domain.Extensions;

namespace NTS.Domain.Setup.Entities;

public class EnduranceEvent : DomainEntity, IParent<Official>, IParent<Competition>, IAggregateRoot
{
    public static EnduranceEvent Create(string? place, Country? country)
    {
        return new(place, country);
    }

    public static EnduranceEvent Update(
        int id,
        string? place,
        Country? country,
        IEnumerable<Competition> competitions,
        IEnumerable<Official> officials
    )
    {
        return new(id, place, country, competitions, officials);
    }

    List<Competition> _competitions = [];
    List<Official> _officials = [];

    [JsonConstructor]
    EnduranceEvent(
        int id,
        string? place,
        Country? country,
        IEnumerable<Competition> competitions,
        IEnumerable<Official> officials
    )
        : base(id)
    {
        Place = Capitalized(nameof(Place), place);
        Country = Required(nameof(Country), country);
        _competitions = competitions.ToList();
        _officials = officials.ToList();
    }

    EnduranceEvent(string? place, Country? country)
        : this(GenerateId(), place, country, [], []) { }

    public string Place { get; }
    public Country Country { get; }
    public IReadOnlyList<Official> Officials => _officials.AsReadOnly();
    public IReadOnlyList<Competition> Competitions => _competitions.AsReadOnly();

    public void Add(Competition competition)
    {
        _competitions.Add(competition);
    }

    public void Update(Competition competition)
    {
        _competitions.Update(competition);
    }

    public void Remove(Competition competition)
    {
        _competitions.Remove(competition);
    }

    public void Add(Official official)
    {
        ValidateRole(official);

        _officials.Add(official);
    }

    public void Update(Official official)
    {
        // TODO: fix check for roles
        _officials.Update(official);
    }

    public void Remove(Official official)
    {
        _officials.Remove(official);
    }

    public override string ToString()
    {
        return Combine(Place, Country);
    }

    static string Capitalized(string name, string? value)
    {
        Required(name, value);

        var character = value.First();
        if (string.IsNullOrEmpty(value) || !char.IsUpper(character))
        {
            throw new DomainException(name, $"Has to be Capital case");
        }
        return value;
    }

    void ValidateRole(Official member)
    {
        var role = member.Role;
        if (member.IsUniqueRole())
        {
            var existing = _officials.FirstOrDefault(x => x.Role == role);
            if (existing != null && existing != member)
            {
                var description = member.Role.GetDescription();
                //TODO: Notification should localize the templates and arguments separately
                var message = string.Format("Official '{0}' already exists", description);
                throw new DomainException(nameof(Official.Role), message);
            }
        }
    }
}
