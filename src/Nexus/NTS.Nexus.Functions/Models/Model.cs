namespace NTS.Nexus.Functions.Models;

public record Model
{
    public string TenantId { get; set; } = default!;
    public int Id { get; set; }
    public string Name { get; set; } = default!;
}
