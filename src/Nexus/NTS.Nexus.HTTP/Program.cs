using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson.Serialization;
using Not.Domain.Base;
using NTS.Domain.Core.Aggregates;
using NTS.Nexus.HTTP.Functions.Archive;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services.AddSingleton<ArchiveRepository>();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

// TODO investigate ImmutableTypeClassMapConvention
BsonClassMap.RegisterClassMap<ArchiveEntry>(classMap =>
{
    classMap.AutoMap();
    classMap.MapProperty(c => c.EnduranceEvent);
    classMap.MapProperty(c => c.Officials);
    classMap.MapProperty(c => c.Rankings);
});
BsonClassMap.RegisterClassMap<Official>(classMap =>
{
    classMap.AutoMap();
    classMap.MapProperty(c => c.Role);
});

BsonClassMap.RegisterClassMap<AggregateRoot>(classMap =>
{
    classMap.AutoMap();
    classMap.MapProperty(c => c.Id);
});

builder.Build().Run();
