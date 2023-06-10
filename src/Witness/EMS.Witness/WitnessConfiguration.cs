﻿using Core;
using Core.Application;
using Core.Application.Services;
using Core.Domain;
using Core.Localization;
using EMS.Witness.Services;
using EMS.Witness.Platforms.Services;
using EMS.Witness.Rpc;
using Core.Application.Rpc;
using Microsoft.Extensions.DependencyInjection;

namespace EMS.Witness;

public static class WitnessConfiguration
{
    public static IServiceCollection AddWitnessServices(this IServiceCollection services)
    {
        var assemblies = CoreConstants.Assemblies
            .Concat(LocalizationConstants.Assemblies)
            .Concat(DomainConstants.Assemblies)
            .Concat(CoreApplicationConstants.Assemblies)
            .Concat(WitnessConstants.Assemblies)
            .ToArray();

        services.AddHttpClient<IRpcService, RpcService>(client => client.Timeout = TimeSpan.FromSeconds(5));
        services
            .AddCore(assemblies)
            .AddSingleton(new Toaster())
            .AddSingleton<IToaster>(x => x.GetRequiredService<Toaster>())
            .AddTransient<IPermissionsService, PermissionsService>()
            .AddSingleton<WitnessState>()
            .AddSingleton<IWitnessState>(provider => provider.GetRequiredService<WitnessState>())
            .AddTransient<IDateService, DateService>()
            .AddSingleton<IStartlistClient, StartlistClient>()
            .AddSingleton<IRpcClient>(p => p.GetRequiredService<IStartlistClient>())
            .AddSingleton<IArrivelistClient, ArrivelistClient>()
            .AddSingleton<IRpcClient>(x => x.GetRequiredService<IArrivelistClient>());

        return services;
    }
}
