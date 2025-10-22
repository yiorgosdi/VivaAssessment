namespace VivaAssessment.Application.Dto;

public sealed class CountryDto
{
    public string CommonName { get; set; } = default!;
    public string Capital { get; set; } = default!;
    public IReadOnlyList<string> Borders { get; set; } = Array.Empty<string>(); 
}
