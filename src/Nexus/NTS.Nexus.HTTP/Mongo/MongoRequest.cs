using NTS.Nexus.HTTP.Common;

namespace NTS.Nexus.HTTP.Mongo;

public record MongoRequest : ITenantAware
{
    public MongoRequest(string tenantId)
    {
        TenantId = tenantId;
    }

    public string TenantId { get; }
}
