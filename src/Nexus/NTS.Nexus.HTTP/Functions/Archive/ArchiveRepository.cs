using MongoDB.Driver;
using Not.Application.CRUD.Ports;
using NTS.Nexus.HTTP.Mongo;
using NTS.Storage.Documents.EnduranceEvents;
using NTS.Storage.Documents.EnduranceEvents.Models;

namespace NTS.Nexus.HTTP.Functions.Archive;

public class ArchiveRepository : MongoRepository<EnduranceEventDocument>, IArchiveRepository
{
    public ArchiveRepository() : base("nts", "archive")
    {
    }

    public async Task<IEnumerable<RankingEntryModel>> GetPerformances(int horseId)
    {
        return await Collection
            .Aggregate()
            .Match(x => x.Rankings
                .Any(y => y.Entries
                    .Any(z => z.Participation.Combination.Horse.Id == horseId)))
            .Project(x => x.Rankings
                .SelectMany(y => y.Entries)
                .First(z => z.Participation.Combination.Horse.Id == horseId))
            .ToListAsync();
    }
}

public interface IArchiveRepository : IRepository<EnduranceEventDocument>
{
    Task<IEnumerable<RankingEntryModel>> GetPerformances(int horseId);
}
