using Microsoft.AspNetCore.Mvc;
using RasyonetInternshipApi.DTOs;
using RasyonetInternshipApi.Services;

namespace RasyonetInternshipApi.Controllers;

[ApiController]
[Route("api/stocks")]
public class StocksController : ControllerBase
{
    private readonly IStockService _stockService;

    public StocksController(IStockService stockService)
    {
        _stockService = stockService;
    }

    [HttpGet]
    public async Task<ActionResult<List<StockResponse>>> GetAll()
    {
        return Ok(await _stockService.GetAllAsync());
    }

    [HttpGet("{symbol}")]
    public async Task<ActionResult<StockResponse>> GetBySymbol(string symbol)
    {
        var result = await _stockService.GetBySymbolAsync(symbol);
        return ToActionResult(result);
    }

    [HttpPost]
    public async Task<ActionResult<StockResponse>> Add(CreateStockRequest request)
    {
        var result = await _stockService.AddAsync(request);

        if (result.Status == ServiceResultStatus.Success && result.Value is not null)
        {
            return CreatedAtAction(nameof(GetBySymbol), new { symbol = result.Value.Symbol }, result.Value);
        }

        return ToActionResult(result);
    }

    [HttpDelete("{symbol}")]
    public async Task<IActionResult> Delete(string symbol)
    {
        var result = await _stockService.DeleteAsync(symbol);

        return result.Status switch
        {
            ServiceResultStatus.Success => NoContent(),
            ServiceResultStatus.Invalid => BadRequest(new { error = result.ErrorMessage }),
            ServiceResultStatus.NotFound => NotFound(new { error = result.ErrorMessage }),
            _ => StatusCode(StatusCodes.Status500InternalServerError, new { error = "Unexpected error." })
        };
    }

    [HttpPost("{symbol}/refresh")]
    public async Task<ActionResult<StockResponse>> Refresh(string symbol)
    {
        var result = await _stockService.RefreshAsync(symbol);
        return ToActionResult(result);
    }

    private ActionResult<StockResponse> ToActionResult(ServiceResult<StockResponse> result)
    {
        return result.Status switch
        {
            ServiceResultStatus.Success => Ok(result.Value),
            ServiceResultStatus.Invalid => BadRequest(new { error = result.ErrorMessage }),
            ServiceResultStatus.NotFound => NotFound(new { error = result.ErrorMessage }),
            ServiceResultStatus.Conflict => Conflict(new { error = result.ErrorMessage }),
            ServiceResultStatus.ExternalApiError => StatusCode(StatusCodes.Status502BadGateway, new { error = result.ErrorMessage }),
            _ => StatusCode(StatusCodes.Status500InternalServerError, new { error = "Unexpected error." })
        };
    }
}
