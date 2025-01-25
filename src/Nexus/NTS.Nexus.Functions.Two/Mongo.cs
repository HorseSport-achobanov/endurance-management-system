using System;
using System.IO;
using System.Security.Authentication;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace NTS.Nexus.Functions.Two;

public static class Mongo
{
    [FunctionName("mongo-get")]
    public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest request,
        ILogger log
    )
    {
        log.LogInformation("Mongo POST function processing a request.");

        try
        {
            var connectionString = @"mongodb://nts-mongo-dev:t4aX66O4VMIvO4vnLvMUEP3sVt8tfcAM651094Xl1WRzv1VsQY9qI48RTb7elIW7kEIt8AcJHfLPACDbrAqJEg==@nts-mongo-dev.mongo.cosmos.azure.com:10255/?ssl=true&retrywrites=false&replicaSet=globaldb&maxIdleTimeMS=120000&appName=@nts-mongo-dev@";
            var settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
            settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
            var mongoClient = new MongoClient(settings);

            var requestBody = await new StreamReader(request.Body).ReadToEndAsync();
            var body = JsonConvert.DeserializeObject<Model>(requestBody);

            mongoClient.GetDatabase("test").GetCollection<Model>("models").InsertOne(body);

            log.LogInformation("Mongo POST function completed processing");
            return new OkResult();
        }
        catch (Exception ex)
        {
            log.LogError(ex, "function failed");
            return new InternalServerErrorResult();
        }
    }
}

public record Model
{
    public int Id { get; set; }
    public string Name { get; set; }
}
