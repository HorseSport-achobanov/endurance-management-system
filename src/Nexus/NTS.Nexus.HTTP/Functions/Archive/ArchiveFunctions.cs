using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Not.Application.CRUD.Ports;
using Not.Serialization;
using NTS.Domain.Core.Aggregates;
using NTS.Storage.Documents.EnduranceEvents;

namespace NTS.Nexus.HTTP.Functions.Archive;

public class ArchiveFunctions
{
    readonly ILogger<ArchiveFunctions> _logger;
    readonly IRepository<EnduranceEventDocument> _archive;

    public ArchiveFunctions(ILogger<ArchiveFunctions> logger, IRepository<EnduranceEventDocument> archive)
    {
        _logger = logger;
        _archive = archive;
    }

    [Function("archive")]
    public async Task<IActionResult> Archive([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest request)
    {
        _logger.LogInformation("C# HTTP 'ArchiveFunctions.Insert' processing a request.");

        var requestBody = await new StreamReader(request.Body).ReadToEndAsync();
        var entry = requestBody.FromJson<ArchiveEntry>();
        var document = new EnduranceEventDocument(entry.EnduranceEvent, entry.Officials, entry.Rankings);
        await _archive.Create(document);

        return new OkObjectResult($"Archived event {entry.EnduranceEvent}");
    }

    [Function("archive-list")]
    public async Task<IActionResult> ListArchive([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest request)
    {
        _logger.LogInformation($"C# HTTP 'ArchiveFunctions.List' processing '{request}'.");

        var result = await _archive.ReadAll();

        return new OkObjectResult(result);
    }
}
