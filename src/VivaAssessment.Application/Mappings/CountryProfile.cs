using AutoMapper;
using VivaAssessment.Application.Dto;
using VivaAssessment.Domain.Entities;

namespace VivaAssessment.Application.Mappings;

public sealed class CountryProfile : Profile
{
    public CountryProfile()
    {
        CreateMap<Country, CountryDto>()
            .ForMember(d => d.Borders, o => o.MapFrom(s => s.Borders ?? new List<string>()))
            .ReverseMap();
    }
}