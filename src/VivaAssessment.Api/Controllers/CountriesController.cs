using Microsoft.AspNetCore.Mvc;
using VivaAssessment.Application.Abstractions;
using VivaAssessment.Application.Dto;

namespace VivaAssessment.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class CountriesController(ICountryService countryService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CountryDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<IReadOnlyList<CountryDto>>> GetAll(CancellationToken ct = default)
    {
        var result = await countryService.GetAllCountriesAsync(ct);
        return (result.Count > 0) ? Ok(result) : NoContent();
    }
}