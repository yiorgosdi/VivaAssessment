using System.Net.Http.Json;
using VivaAssessment.Application.Abstractions;
using VivaAssessment.Application.RestCountries;
using VivaAssessment.Infrastructure.Exceptions;

namespace VivaAssessment.Infrastructure.RestCountries;

public sealed class RestCountriesClient : IRestCountriesClient
{
    private readonly HttpClient _http;
    public RestCountriesClient(HttpClient http) => _http = http;

    public async Task<IReadOnlyList<RestCountryDto>> GetAllCountriesAsync(CancellationToken ct = default)
    {
        try
        {
            var api = await _http.GetFromJsonAsync<List<ApiModel>>(
            "v3.1/all?fields=name,capital,borders", ct) ?? [];

            return api.Select(x => new RestCountryDto(
                    x.name?.common ?? string.Empty,
                    x.capital ?? [],
                    x.borders ?? []
                )
            ).ToList();
        }
        catch (TaskCanceledException ex)
        {
            throw new ExternalServiceException(
                serviceName: "RestCountries",
                message: "RestCountries API request timed out.",
                inner: ex
            );
        }
        catch (HttpRequestException ex)
        {
            throw new ExternalServiceException("RestCountries", "HTTP error while calling RestCountries.", null, ex);
        }
    }

    // for internal decoding (Infrastructure layer)
    private sealed class ApiModel
    {
        public NameObj? name { get; set; }
        public List<string>? capital { get; set; }
        public List<string>? borders { get; set; }
        public sealed class NameObj { public string? common { get; set; } }
    }
}
