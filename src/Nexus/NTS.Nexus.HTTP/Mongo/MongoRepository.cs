using MongoDB.Driver;
using System.Security.Authentication;
using Not.Domain;
using Not.Application.CRUD.Ports;
using NTS.Storage.Documents;

namespace NTS.Nexus.HTTP.Mongo;

public abstract class MongoRepository<T> : IRepository<T>
    where T : Document, IAggregateRoot
{
    readonly IMongoCollection<T> _collection;

    public MongoRepository(string db, string collection)
    {
        var connectionString = @"mongodb://nts-mongo-dev:t4aX66O4VMIvO4vnLvMUEP3sVt8tfcAM651094Xl1WRzv1VsQY9qI48RTb7elIW7kEIt8AcJHfLPACDbrAqJEg==@nts-mongo-dev.mongo.cosmos.azure.com:10255/?ssl=true&retrywrites=false&replicaSet=globaldb&maxIdleTimeMS=120000&appName=@nts-mongo-dev@";
        var settings = MongoClientSettings.FromUrl(new MongoUrl(connectionString));
        settings.SslSettings = new SslSettings() { EnabledSslProtocols = SslProtocols.Tls12 };
        _collection = new MongoClient(settings).GetDatabase(db).GetCollection<T>(collection);
    }

    public async Task Create(T document)
    {
        try
        {
            await _collection.InsertOneAsync(document);
        }
        catch (MongoWriteException ex)
        {
            if (ex.WriteError.Code == 11000)
            {
                throw new Exception($"Could not insert. Document with ID '{document.Id}' already exists", ex);
            }
            else
            {
                throw;
            }
        }
    }

    public Task<T?> Read(Predicate<T> filter)
    {
        throw new NotImplementedException();
    }

    public Task<T?> Read(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<T>> ReadAll()
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<T>> ReadAll(Predicate<T> filter)
    {
        throw new NotImplementedException();
    }

    public Task Update(T entity)
    {
        throw new NotImplementedException();
    }

    public Task Delete(int id)
    {
        throw new NotImplementedException();
    }

    public Task Delete(T entity)
    {
        throw new NotImplementedException();
    }

    public Task Delete(Predicate<T> filter)
    {
        throw new NotImplementedException();
    }

    public Task Delete(IEnumerable<T> entities)
    {
        throw new NotImplementedException();
    }
}
