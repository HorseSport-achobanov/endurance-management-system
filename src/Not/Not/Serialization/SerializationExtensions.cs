﻿using JsonNet.PrivatePropertySetterResolver;
using Newtonsoft.Json;

namespace Not.Serialization;

public static class SerializationExtensions
{
    static readonly JsonSerializerSettings SETTINGS = new()
    {
        ContractResolver = new PrivatePropertySetterResolver(),
        ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
        Formatting = Newtonsoft.Json.Formatting.Indented,
        TypeNameHandling = TypeNameHandling.Auto,
        ReferenceLoopHandling = ReferenceLoopHandling.Serialize,
    };

    public static void AddConverter<T>(T converter)
        where T : JsonConverterBase
    {
        _converters.Add(converter);
        SETTINGS.Converters.Add(converter);
    }

    public static string ToJson(this object obj)
    {
        var result = JsonConvert.SerializeObject(obj, SETTINGS);
        ResetConverters();
        return result;
    }

    public static T FromJson<T>(this string json)
        where T : class
    {
        var result = JsonConvert.DeserializeObject<T>(json, SETTINGS);
        if (result == default)
        {
            throw new Exception($"Cannot serialize '{json}' to type of '{typeof(T)}'");
        }
        ResetConverters();
        return result;
    }

    static List<JsonConverterBase> _converters = [];

    static void ResetConverters()
    {
        foreach (var converter in _converters)
        {
            converter.Reset();
        }
    }
}

public abstract class JsonConverterBase : JsonConverter
{
    public abstract void Reset();
}
