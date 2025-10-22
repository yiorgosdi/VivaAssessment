namespace VivaAssessment.Application.RestCountries;

public sealed record RestCountryDto(
    string Name,
    IReadOnlyList<string> Capital,
    IReadOnlyList<string> Borders
);
