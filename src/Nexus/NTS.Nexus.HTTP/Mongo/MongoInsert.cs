using Not.Domain;

namespace NTS.Nexus.HTTP.Mongo;

public record MongoInsert<T> : MongoRequest
    where T : IAggregateRoot
{
    public MongoInsert(string tenantId, T document) : base(tenantId)
    {
        Content = document;
    }

    public T Content { get; }
}
