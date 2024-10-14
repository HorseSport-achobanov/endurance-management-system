﻿using Not.Events;
using Not.Exceptions;
using System.Collections;

namespace Not.Structures;

public class ObservableList<T> : IReadOnlyList<T>
    where T : IIdentifiable
{
    Dictionary<int, T> _dictionary = [];

    public SyncEventManager Changed { get; } = new();

    public void AddOrReplace(T item)
    {
        GuardHelper.ThrowIfDefault(item);

        if (!_dictionary.TryAdd(item.Id, item))
        {
            _dictionary[item.Id] = item;
        }

        Changed.Emit();
    }

    public bool Remove(T item)
    {
        GuardHelper.ThrowIfDefault(item);
        var result = _dictionary.Remove(item.Id);
        Changed.Emit();
        return result;
    }

    public bool Remove(int id)
    {
        var result = _dictionary.Remove(id);
        Changed.Emit();
        return result;
    }

    public void AddRange(IEnumerable<T> items)
    {
        _dictionary = items.ToDictionary(x => x.Id, x => x);
        Changed.Emit();
    }

    public T this[int index]
    {
        get
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, _dictionary.Count);

            var count = 0;
            foreach (var item in this)
            {
                if (count++ == index)
                {
                    return item;
                }
            }
            // Should never be reached
            throw new Exception();
        }
    }

    public int Count => _dictionary.Count;

    public IEnumerator<T> GetEnumerator()
    {
        return _dictionary.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
