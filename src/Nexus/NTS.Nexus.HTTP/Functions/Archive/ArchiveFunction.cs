using System.Reflection.Metadata;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Not.Application.CRUD.Ports;
using Not.Serialization;
using NTS.Domain.Core.Aggregates;
using NTS.Storage.Documents.EnduranceEvents;

namespace NTS.Nexus.HTTP.Functions.Archive;

public class ArchiveFunction
{
    readonly ILogger<ArchiveFunction> _logger;
    readonly IRepository<EnduranceEventDocument> _archive;

    public ArchiveFunction(ILogger<ArchiveFunction> logger, IRepository<EnduranceEventDocument> archive)
    {
        _logger = logger;
        _archive = archive;
    }

    [Function("archive")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest request)
    {
        _logger.LogInformation("C# HTTP trigger function processing a request.");

        var requestBody = await new StreamReader(request.Body).ReadToEndAsync();
        var entry = requestBody.FromJson<ArchiveEntry>();
        var document = new EnduranceEventDocument(entry.EnduranceEvent, entry.Officials, entry.Rankings);
        await _archive.Create(document);

        return new OkObjectResult($"Archived event {entry.EnduranceEvent}");
    }
}
