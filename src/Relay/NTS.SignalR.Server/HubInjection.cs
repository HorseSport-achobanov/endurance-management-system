using Microsoft.AspNetCore.SignalR;
using Not.Injection;
using Not.Localization;
using Not.Serialization;
using NTS.ACL.Handshake;
using NTS.SignalR.Server.Middleware;
using NTS.Storage;

namespace NTS.SignalR.Server;

public static class HubInjection
{
    public static IServiceCollection ConfigureHub(this IServiceCollection services)
    {
        services
            .AddSignalR(options => {
                options.EnableDetailedErrors = true;
                options.AddFilter<ExceptionHandlingHubFilter>();
            })
            .AddNewtonsoftJsonProtocol(x =>
            {
                x.PayloadSerializerSettings = SerializationExtensions.SETTINGS;
            });

        services
            .AddHostedService<NetworkBroadcastService>()
            .AddDummyLocalizer()
            .RegisterConventionalServices()
            .ConfigureStorage();

        return services;
    }
}
