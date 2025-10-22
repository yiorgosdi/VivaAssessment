using Microsoft.AspNetCore.Mvc;
using VivaAssessment.Domain.Entities;

namespace VivaAssessment.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class NumbersController : ControllerBase
{
    public sealed record SecondLargestResponse(string Message, int Value);

    [HttpPost("second-largest")]
    [ProducesResponseType(typeof(SecondLargestResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public ActionResult<SecondLargestResponse> SecondLargest([FromBody] RequestObj req)
    {
        var distinct = req.RequestArrayObj!
            .Distinct()
            .OrderByDescending(x => x)
            .ToList();

        if (distinct.Count < 2)
        {
            return ValidationProblem(new ValidationProblemDetails
            {
                Title = "Invalid data",
                Detail = "Array must contain at least two distinct integers, separated with comma.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        return Ok(new SecondLargestResponse("the second largest number:", distinct[1]));
    }
}