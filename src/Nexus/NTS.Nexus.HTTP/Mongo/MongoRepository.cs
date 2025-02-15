using MongoDB.Driver;
using System.Security.Authentication;
using Not.Domain;

namespace NTS.Nexus.HTTP.Mongo;

public abstract class MongoRepository<T, TContent>
    where T : MongoDocument
    where TContent : IAggregateRoot
{
    readonly IMongoCollection<T> _collection;

    public MongoRepository(string db, string collection)
    {
        var connectionString = @"mongodb://nts-mongo-dev:t4aX66O4VMIvO4vnLvMUEP3sVt8tfcAM651094Xl1WRzv1VsQY9qI48RTb7elIW7kEIt8AcJHfLPACDbrAqJEg==@nts-mongo-dev.mongo.cosmos.azure.com:10255/?ssl=true&retrywrites=false&replicaSet=globaldb&maxIdleTimeMS=120000&appName=@nts-mongo-dev@";
        var settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
        settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
        _collection = new MongoClient(settings).GetDatabase(db).GetCollection<T>(collection);
    }

    protected abstract T CreateDocument(string tenantId, TContent content);

    public async Task Create(MongoInsert<TContent> insert)
    {
        var document = CreateDocument(insert.TenantId, insert.Content);
        await _collection.InsertOneAsync(document);
    }
}
