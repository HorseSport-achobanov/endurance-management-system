﻿using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace EnduranceJudge.Domain;

public static class DomainServices
{
    public static IServiceCollection AddDomain(this IServiceCollection services, Assembly[] assemblies)
    {
        return services;
    }
}
