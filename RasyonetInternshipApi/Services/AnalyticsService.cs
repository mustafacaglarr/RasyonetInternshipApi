using RasyonetInternshipApi.DTOs;
using RasyonetInternshipApi.Models;
using RasyonetInternshipApi.Repositories;

namespace RasyonetInternshipApi.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly IStockRepository _stockRepository;

    public AnalyticsService(IStockRepository stockRepository)
    {
        _stockRepository = stockRepository;
    }

    public async Task<ServiceResult<List<StockResponse>>> GetTopGainersAsync(int count)
    {
        if (count <= 0 || count > 50)
        {
            return ServiceResult<List<StockResponse>>.Invalid("Count must be between 1 and 50.");
        }

        var stocks = await _stockRepository.GetTopGainersAsync(count);
        return ServiceResult<List<StockResponse>>.Success(stocks.Select(ToResponse).ToList());
    }

    private static StockResponse ToResponse(Stock stock)
    {
        return new StockResponse
        {
            Id = stock.Id,
            Symbol = stock.Symbol,
            CompanyName = stock.CompanyName,
            CurrentPrice = stock.CurrentPrice,
            PreviousClosePrice = stock.PreviousClosePrice,
            ChangePercent = stock.ChangePercent,
            LastUpdatedAt = stock.LastUpdatedAt
        };
    }
}
