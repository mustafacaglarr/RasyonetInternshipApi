using Microsoft.AspNetCore.Mvc;
using RasyonetInternshipApi.DTOs;
using RasyonetInternshipApi.Services;

namespace RasyonetInternshipApi.Controllers;

[ApiController]
[Route("api/analytics")]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;

    public AnalyticsController(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    [HttpGet("top-gainers")]
    public async Task<ActionResult<List<StockResponse>>> GetTopGainers([FromQuery] int count = 5)
    {
        var result = await _analyticsService.GetTopGainersAsync(count);

        return result.Status switch
        {
            ServiceResultStatus.Success => Ok(result.Value),
            ServiceResultStatus.Invalid => BadRequest(new { error = result.ErrorMessage }),
            _ => StatusCode(StatusCodes.Status500InternalServerError, new { error = "Unexpected error." })
        };
    }
}
