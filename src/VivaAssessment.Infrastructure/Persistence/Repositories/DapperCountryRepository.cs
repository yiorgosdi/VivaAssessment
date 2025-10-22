using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Data.Common;
using VivaAssessment.Application.Abstractions;
using VivaAssessment.Domain.Entities;

namespace VivaAssessment.Infrastructure.Persistence.Repositories;

public sealed class DapperCountryRepository : ICountryRepository
{
    private readonly SqlConnection _db;
    public DapperCountryRepository(DbConnection db) => _db = (SqlConnection?)db;

    public async Task<List<Country>> GetAllAsync(CancellationToken ct = default)
    {
        const string sql = @"
SELECT c.Id, c.CommonName, c.Capital, b.BorderCode
FROM dbo.Countries c
LEFT JOIN dbo.CountryBorders b ON b.CountryId = c.Id
ORDER BY c.CommonName;";

        var dict = new Dictionary<int, Country>();

        var _ = await _db.QueryAsync<CountryRow, BorderRow, Country>(
             new CommandDefinition(sql, cancellationToken: ct),
             (c, b) =>
             {
                 if (!dict.TryGetValue(c.Id, out var agg))
                 {
                     agg = new Country
                     {
                         CommonName = c.CommonName,
                         Capital = c.Capital,
                         Borders = new List<string>()
                     };
                     dict.Add(c.Id, agg);
                 }
                 if (!string.IsNullOrWhiteSpace(b?.BorderCode))
                     agg.Borders.Add(b.BorderCode);
                 return agg;
             },
             splitOn: "BorderCode");

        foreach (var kv in dict)
            kv.Value.Borders = kv.Value.Borders
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => x.Trim().ToUpperInvariant())
                .Distinct()
                .ToList();

        return dict.Values.ToList();
    }

    public async Task UpsertManyAsync(IEnumerable<Country> countries, CancellationToken ct = default)
    {
        if (_db.State != ConnectionState.Open)
            await _db.OpenAsync(ct);

        await using var tx = await _db.BeginTransactionAsync(ct);

        try
        {
            //merge in countries table and get id. 
            const string mergeCountrySql = @"
MERGE dbo.Countries AS t
USING (VALUES (@CommonName, @Capital)) AS s(CommonName, Capital)
  ON t.CommonName = s.CommonName
WHEN MATCHED THEN
  UPDATE SET Capital = s.Capital
WHEN NOT MATCHED THEN
  INSERT (CommonName, Capital) VALUES (s.CommonName, s.Capital)
OUTPUT inserted.Id;";

            //delete old borders for the specific CointryId. 
            const string deleteBordersSql = @"
DELETE FROM dbo.CountryBorders WHERE CountryId = @CountryId;";

            //insert one-by-one the current. 
            const string insertBorderSql = @"
INSERT INTO dbo.CountryBorders (CountryId, BorderCode)
VALUES (@CountryId, @BorderCode);";

            foreach (var c in countries)
            {
                // 1) Upsert country & get Id
                var countryId = await _db.ExecuteScalarAsync<int>(
                    new CommandDefinition(
                        mergeCountrySql,
                        new { c.CommonName, Capital = c.Capital ?? string.Empty },
                        transaction: tx,
                        cancellationToken: ct));

                // 2) cleaning old borders
                await _db.ExecuteAsync(
                    new CommandDefinition(
                        deleteBordersSql,
                        new { CountryId = countryId },
                        transaction: tx,
                        cancellationToken: ct));

                // 3) insert current borders (distinct + trim = cleaner)
                if (c.Borders != null)
                {
                    foreach (var code in c.Borders.Where(b => !string.IsNullOrWhiteSpace(b)).Select(b => b.Trim()).Distinct())
                    {
                        await _db.ExecuteAsync(
                            new CommandDefinition(
                                insertBorderSql,
                                new { CountryId = countryId, BorderCode = code },
                                transaction: tx,
                                cancellationToken: ct));
                    }
                }
            }

            await tx.CommitAsync(ct);
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
    }

    private sealed record CountryRow
    {
        public int Id { get; init; }
        public string CommonName { get; init; } = default!;
        public string Capital { get; init; } = default!;
    }

    private sealed record BorderRow
    {
        public string? BorderCode { get; init; }
    }
}
