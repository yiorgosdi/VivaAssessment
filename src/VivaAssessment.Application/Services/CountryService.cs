using AutoMapper;
using VivaAssessment.Application.Abstractions;
using VivaAssessment.Application.Constants;
using VivaAssessment.Application.Dto;
using VivaAssessment.Domain.Entities;

namespace VivaAssessment.Application.Services;

public sealed class CountryService : ICountryService
{
    private readonly ICacheService _cache;
    private readonly ICountryRepository _repo;
    private readonly IRestCountriesClient _client;
    private readonly IMapper _mapper;

    private static readonly TimeSpan Ttl = TimeSpan.FromSeconds(60);

    public CountryService(
        ICacheService cache,
        ICountryRepository repo,
        IRestCountriesClient client,
        IMapper mapper)
    {
        _cache = cache;
        _repo = repo;
        _client = client;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<CountryDto>> GetAllCountriesAsync(CancellationToken ct = default)
    {
        // 1) check cache first
        var cached = _cache.Get<IReadOnlyList<CountryDto>>(CacheKeys.AllCountries);
        if (cached is { Count: > 0 }) 
          return cached;

        // 2) Database 
        var dbData = await _repo.GetAllAsync(ct);
        if (dbData is { Count: > 0 })
        {
            // apply materialize
            var fromDb = _mapper.Map<List<CountryDto>>(dbData);
            _cache.Set(CacheKeys.AllCountries, fromDb, Ttl);
            return fromDb;
        }

        // 3) API: fetch>normalize>upsert>return
        var api = await _client.GetAllCountriesAsync(ct);

        var countries = api
            .Where(x => !string.IsNullOrWhiteSpace(x.Name))
            .Select(x => new Country
            {
                CommonName = x.Name!, 
                Capital = x.Capital?.FirstOrDefault() ?? string.Empty, 
                Borders = (x.Borders ?? Enumerable.Empty<string>())
                               .Where(b => !string.IsNullOrWhiteSpace(b))
                               .Select(b => b.Trim().ToUpperInvariant())
                               .Distinct()
                               .ToList()
            })
            .OrderBy(x => x.CommonName)
            .ToList();

        await _repo.UpsertManyAsync(countries, ct);

        var fromApi = _mapper.Map<List<CountryDto>>(countries);
        _cache.Set(CacheKeys.AllCountries, fromApi, Ttl);
        
        return fromApi;
    }
}