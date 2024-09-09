﻿namespace Not.Blazor.TM.Models;

public class NotListModel
{
    public static IEnumerable<NotListModel<T>> FromEnum<T>()
    {
        var values = Enum.GetValues(typeof(T));
        foreach (var value in values)
        {
            yield return new NotListModel<T>((T)value);
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

    public T Value { get; }
    public string Label { get; }
}
