using NTS.Nexus.HTTP.Mongo;
using NTS.Storage.Documents.EnduranceEvents;

namespace NTS.Nexus.HTTP.Functions.Archive;

public class ArchiveRepository : MongoRepository<EnduranceEventDocument>
{
    public ArchiveRepository() : base("nts", "archive")
    {
    }
}
