namespace NTS.Nexus.Functions.Models;

public record Model
{
    public Guid TenantId { get; set; }
    public int Id { get; set; }
    public string Name { get; set; } = default!;
}
