using Not.Storage.Tenants;
using Not.Structures;

namespace NTS.Storage.Documents;

public class Document : IIdentifiable, ITenantAware
{
    public Document(int id)
    {
        Id = id;    
    }

    public int Id { get; init; }
    public string TenantId { get; set; } = "nts";
}
