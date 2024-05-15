﻿using Newtonsoft.Json;

namespace NTS.Domain.Setup.Entities;
public class Loop : DomainEntity, IParent
{
    public static Loop Create(double distance) => new (distance);
    public static Loop Update(int id, double distance) => new(id, distance);

    [JsonConstructor]
    public Loop(int id, double distance) : this(distance)
    {
        Id = id;
    }
    public Loop(double distance)
    {
        if (distance <= 0)
        {
            throw new DomainException(nameof(Distance), "Distance cannot be zero or less.");
        }

        Distance = distance;
    }
    public double Distance { get; set; }

    public override string ToString() 
    {
        var phase = "Loop".Localize();
        var sb = new StringBuilder();
        sb.Append($"{phase} -> {Distance}km long ");
        return sb.ToString();
    }
}
