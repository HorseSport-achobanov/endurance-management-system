﻿using Newtonsoft.Json;

namespace NTS.Domain.Objects;

public class Person : IComparable<Person>
{
    public static Person? Create(string? names)
    {
        if (string.IsNullOrWhiteSpace(names))
        {
            return null;
        }
        return new Person(names.Split(DELIMITER, StringSplitOptions.RemoveEmptyEntries));
    }

    public static implicit operator string(Person member)
    {
        return member.ToString();
    }

    internal static string DELIMITER = " ";

    [JsonConstructor]
    public Person(string[] names)
    {
        Names = names;
    }

    public string[] Names { get; private set; } = [];

    public override string ToString()
    {
        return string.Join(DELIMITER, Names);
    }
}
