using Microsoft.Extensions.DependencyInjection;
using VivaAssessment.Application.Abstractions;
using VivaAssessment.Application.Mappings;
using VivaAssessment.Application.Services;

namespace VivaAssessment.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Services (application layer)
        services.AddScoped<ICountryService, CountryService>();

        // AutoMapper: loads profiles from assembly (application layer)
        services.AddAutoMapper(typeof(CountryProfile).Assembly);

        return services;
    }
}
