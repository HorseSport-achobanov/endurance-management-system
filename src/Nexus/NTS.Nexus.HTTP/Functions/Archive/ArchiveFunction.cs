using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Not.Serialization;
using NTS.Domain.Core.Aggregates;
using NTS.Nexus.HTTP.Mongo;

namespace NTS.Nexus.HTTP.Functions.Archive;

public class ArchiveFunction
{
    readonly ILogger<ArchiveFunction> _logger;
    readonly ArchiveRepository _archive;

    public ArchiveFunction(ILogger<ArchiveFunction> logger, ArchiveRepository archive)
    {
        _logger = logger;
        _archive = archive;
    }

    [Function("nts-archive")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest request)
    {
        _logger.LogInformation("C# HTTP trigger function processing a request.");

        var requestBody = await new StreamReader(request.Body).ReadToEndAsync();
        var entry = requestBody.FromJson<ArchiveEntry>();
        var insert = new MongoInsert<ArchiveEntry>(entry.TenantId, entry);
        await _archive.Create(insert);

        return new OkObjectResult($"Archived event {entry.EnduranceEvent}");
    }
}
