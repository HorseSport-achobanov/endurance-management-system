using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Not.Serialization;
using NTS.Domain.Core.Aggregates;
using NTS.Storage.Documents.EnduranceEvents;

namespace NTS.Nexus.HTTP.Functions.Archive;

public class ArchiveFunctions
{
    readonly ILogger<ArchiveFunctions> _logger;
    readonly IArchiveRepository _archive;

    public ArchiveFunctions(IArchiveRepository archive, ILogger<ArchiveFunctions> logger)
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

    [Function("archive-query-by-horse")]
    public async Task<IActionResult> QueryPerformances([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "archive/horse/{id:int}")] HttpRequest request, int id)
    {
        _logger.LogInformation("C# HTTP '{FunctionName}' processing '{request}'", $"{nameof(ArchiveFunctions)}.{nameof(QueryPerformances)}", request);

        var performances = await _archive.GetPerformances(id);

        return new OkObjectResult(performances);
    }
}
