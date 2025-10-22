using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Data.Common;
using VivaAssessment.Application.Abstractions;
using VivaAssessment.Infrastructure.Caching;
using VivaAssessment.Infrastructure.Persistence.Repositories;

namespace VivaAssessment.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration cfg)
    {
        var baseAddress = cfg["RestCountries:BaseAddress"] ?? "https://restcountries.com/";
        services.AddHttpClient<IRestCountriesClient, RestCountries.RestCountriesClient>(http =>
        {
            http.BaseAddress = new Uri(baseAddress);
            http.Timeout = TimeSpan.FromSeconds(10);//in case RestCountries Api not respond in 10" 
        });

        var cs = cfg.GetConnectionString("SqlServer")
                 ?? throw new InvalidOperationException("Missing connection string 'SqlServer'.");
        services.AddScoped<DbConnection>(_ => new SqlConnection(cs));

        services.AddScoped<ICountryRepository, DapperCountryRepository>();
        
        services.AddMemoryCache();
        services.AddSingleton<ICacheService, MemoryCacheService>();
        return services;
    }    
}
