﻿using Not.Extensions;

namespace Not.Blazor.TM.Models;

public class NotListModel
{
    public static IEnumerable<NotListModel<T>> FromEnum<T>()
        where T : Enum
    {
        var values = Enum.GetValues(typeof(T));
        foreach (var value in values)
        {
            var enumValue = (T)value;
            yield return new NotListModel<T>(enumValue, enumValue.GetDescription());
        }
    }

    public static IEnumerable<NotListModel<T>> FromEntity<T>(IEnumerable<T> values)
    {
        foreach (var value in values)
        {
            yield return new NotListModel<T>(value);
        }
    }
}

public class NotListModel<T>
{
    public NotListModel(T value, string? label = null)
    {
        Value = value;
        Label = label ?? value!.ToString()!;
    }

    public NotListModel(string label)
    {
        Label = label;
    }

    public T? Value { get; } = default!;
    public string Label { get; }
}