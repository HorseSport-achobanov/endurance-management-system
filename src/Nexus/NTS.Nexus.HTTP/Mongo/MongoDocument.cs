using NTS.Nexus.HTTP.Common;

namespace NTS.Nexus.HTTP.Mongo;

public abstract class MongoDocument : ITenantAware
{
    protected MongoDocument(string tenantId)
    {
        TenantId = tenantId;
    }

    public string TenantId { get; set; }
}
