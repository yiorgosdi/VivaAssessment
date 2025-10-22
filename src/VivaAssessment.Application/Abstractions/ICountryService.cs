using VivaAssessment.Application.Dto;

namespace VivaAssessment.Application.Abstractions;
public interface ICountryService
{
    Task<IReadOnlyList<CountryDto>> GetAllCountriesAsync(CancellationToken ct = default);
}

