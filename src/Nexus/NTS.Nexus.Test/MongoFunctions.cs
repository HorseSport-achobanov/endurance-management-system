using System.Security.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json;
using NTS.Nexus.Functions.Models;

namespace NTS.Nexus.Functions;

public class MongoFunctions
{
    readonly ILogger<MongoFunctions> _logger;

    public MongoFunctions(ILogger<MongoFunctions> logger)
    {
        _logger = logger;
    }

    [Function("mongo-post")]
    public async Task<IActionResult> Post([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest request)
    {
        try
        {
            var connectionString = @"mongodb://nts-mongo-dev:t4aX66O4VMIvO4vnLvMUEP3sVt8tfcAM651094Xl1WRzv1VsQY9qI48RTb7elIW7kEIt8AcJHfLPACDbrAqJEg==@nts-mongo-dev.mongo.cosmos.azure.com:10255/?ssl=true&retrywrites=false&replicaSet=globaldb&maxIdleTimeMS=120000&appName=@nts-mongo-dev@";
            var settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
            settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
            var mongoClient = new MongoClient(settings);

            var requestBody = await new StreamReader(request.Body).ReadToEndAsync();
            var body = JsonConvert.DeserializeObject<Model>(requestBody);
            if (body == null)
            {
                return new BadRequestObjectResult("Invalid request body");
            }

            mongoClient.GetDatabase("test").GetCollection<Model>("models").InsertOne(body);

            _logger.LogInformation("Mongo POST function completed processing");
            return new OkResult();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Mongo POST function failed");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }

    [Function("mongo-get")]
    public async Task<IActionResult> Get([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest request)
    {
        try
        {
            _logger.LogInformation($"Mongo GET processing '{request}'");

            var connectionString = @"mongodb://nts-mongo-dev:t4aX66O4VMIvO4vnLvMUEP3sVt8tfcAM651094Xl1WRzv1VsQY9qI48RTb7elIW7kEIt8AcJHfLPACDbrAqJEg==@nts-mongo-dev.mongo.cosmos.azure.com:10255/?ssl=true&retrywrites=false&replicaSet=globaldb&maxIdleTimeMS=120000&appName=@nts-mongo-dev@";
            var settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
            settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
            var mongoClient = new MongoClient(settings);

            var results = await mongoClient.GetDatabase("test").GetCollection<Model>("models").FindAsync(x => true);
            var models = await results.ToListAsync();

            _logger.LogInformation("Mongo GET function completed processing");
            return new OkObjectResult(models);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Mongo GET function failed");
            return new StatusCodeResult(StatusCodes.Status500InternalServerError);
        }
    }
}
