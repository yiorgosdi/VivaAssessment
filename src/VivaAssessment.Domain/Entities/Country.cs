namespace VivaAssessment.Domain.Entities;

public sealed class Country
{
    public int Id { get; set; }
    public string CommonName { get; set; } = default!;
    public string? Capital { get; set; } = string.Empty; // saving the capital as string.
    public List<string> Borders { get; set; } = new();
}
