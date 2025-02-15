namespace NTS.Nexus.HTTP.Common;

public interface ITenantAware
{
    string TenantId { get; }
}
