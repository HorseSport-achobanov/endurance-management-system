using NTS.Domain.Core.Aggregates;
using NTS.Nexus.HTTP.Mongo;

namespace NTS.Nexus.HTTP.Functions.Archive;

public class ArchiveEntity : MongoDocument
{
    public ArchiveEntity(string tenantId, ArchiveEntry archiveEvent) : base(tenantId)
    {
        Event = archiveEvent;
    }

    public ArchiveEntry Event { get; }
}
