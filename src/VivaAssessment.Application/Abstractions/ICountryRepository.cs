using VivaAssessment.Domain.Entities;

namespace VivaAssessment.Application.Abstractions;
public interface ICountryRepository
{
    Task<List<Country>> GetAllAsync(CancellationToken ct = default);
    Task UpsertManyAsync(IEnumerable<Country> countries, CancellationToken ct = default);
}
