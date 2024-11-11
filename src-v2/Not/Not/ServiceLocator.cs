﻿using Microsoft.Extensions.DependencyInjection;
using Not.Exceptions;
using Not.Injection;
using Not.Startup;

namespace Not;

public class ServiceLocator : IStartupInitializer, ITransientService
{
    public static T Get<T>()
        where T : class
    {
        GuardHelper.ThrowIfDefault(_provider);

        return _provider.GetRequiredService<T>();
    }

    static IServiceProvider? _provider;

    public ServiceLocator(IServiceProvider provider)
    {
        _provider ??= provider;
    }

    public void RunAtStartup()
    {
        // It's just necessary to load ServiceLocator in order to set _provider from DI container
    }
}
