using NTS.Domain.Core.Aggregates;
using NTS.Nexus.HTTP.Mongo;

namespace NTS.Nexus.HTTP.Functions.Archive;

public class ArchiveRepository : MongoRepository<ArchiveEntity, ArchiveEntry>
{
    public ArchiveRepository() : base("nts", "archive")
    {
    }

    protected override ArchiveEntity CreateDocument(string tenantId, ArchiveEntry content)
    {
        return new ArchiveEntity(tenantId, content);
    }
}
