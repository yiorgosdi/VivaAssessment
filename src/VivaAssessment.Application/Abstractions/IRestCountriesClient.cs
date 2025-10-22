using VivaAssessment.Application.RestCountries;

namespace VivaAssessment.Application.Abstractions;
public interface IRestCountriesClient
{
    Task<IReadOnlyList<RestCountryDto>> GetAllCountriesAsync(CancellationToken ct = default);
}