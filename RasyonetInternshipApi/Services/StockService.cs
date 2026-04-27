using RasyonetInternshipApi.DTOs;
using RasyonetInternshipApi.Models;
using RasyonetInternshipApi.Repositories;

namespace RasyonetInternshipApi.Services;

public class StockService : IStockService
{
    private readonly IStockRepository _stockRepository;
    private readonly IFinancialDataService _financialDataService;

    public StockService(IStockRepository stockRepository, IFinancialDataService financialDataService)
    {
        _stockRepository = stockRepository;
        _financialDataService = financialDataService;
    }

    public async Task<List<StockResponse>> GetAllAsync()
    {
        var stocks = await _stockRepository.GetAllAsync();
        return stocks.Select(ToResponse).ToList();
    }

    public async Task<ServiceResult<StockResponse>> GetBySymbolAsync(string symbol)
    {
        var normalizedSymbol = NormalizeSymbol(symbol);
        if (!IsValidSymbol(normalizedSymbol))
        {
            return ServiceResult<StockResponse>.Invalid("Symbol is required and can only contain letters, numbers, dot, or dash.");
        }

        var stock = await _stockRepository.GetBySymbolAsync(normalizedSymbol);
        if (stock is null)
        {
            return ServiceResult<StockResponse>.NotFound("Stock was not found.");
        }

        return ServiceResult<StockResponse>.Success(ToResponse(stock));
    }

    public async Task<ServiceResult<StockResponse>> AddAsync(CreateStockRequest request)
    {
        var normalizedSymbol = NormalizeSymbol(request.Symbol);
        if (!IsValidSymbol(normalizedSymbol) || string.IsNullOrWhiteSpace(request.CompanyName))
        {
            return ServiceResult<StockResponse>.Invalid("Symbol and companyName are required.");
        }

        if (await _stockRepository.ExistsAsync(normalizedSymbol))
        {
            return ServiceResult<StockResponse>.Conflict("Stock already exists in the watchlist.");
        }

        var stock = new Stock
        {
            Symbol = normalizedSymbol,
            CompanyName = request.CompanyName.Trim(),
            LastUpdatedAt = DateTime.UtcNow
        };

        await _stockRepository.AddAsync(stock);
        await _stockRepository.SaveChangesAsync();

        return ServiceResult<StockResponse>.Success(ToResponse(stock));
    }

    public async Task<ServiceResult<bool>> DeleteAsync(string symbol)
    {
        var normalizedSymbol = NormalizeSymbol(symbol);
        if (!IsValidSymbol(normalizedSymbol))
        {
            return ServiceResult<bool>.Invalid("Symbol is required and can only contain letters, numbers, dot, or dash.");
        }

        var stock = await _stockRepository.GetBySymbolAsync(normalizedSymbol);
        if (stock is null)
        {
            return ServiceResult<bool>.NotFound("Stock was not found.");
        }

        await _stockRepository.DeleteAsync(stock);
        await _stockRepository.SaveChangesAsync();

        return ServiceResult<bool>.Success(true);
    }

    public async Task<ServiceResult<StockResponse>> RefreshAsync(string symbol)
    {
        var normalizedSymbol = NormalizeSymbol(symbol);
        if (!IsValidSymbol(normalizedSymbol))
        {
            return ServiceResult<StockResponse>.Invalid("Symbol is required and can only contain letters, numbers, dot, or dash.");
        }

        var stock = await _stockRepository.GetBySymbolAsync(normalizedSymbol);
        if (stock is null)
        {
            return ServiceResult<StockResponse>.NotFound("Stock was not found.");
        }

        FinancialQuote? quote;
        try
        {
            quote = await _financialDataService.GetQuoteAsync(normalizedSymbol);
        }
        catch
        {
            return ServiceResult<StockResponse>.ExternalApiError("Could not fetch stock data from Finnhub.");
        }

        if (quote is null)
        {
            return ServiceResult<StockResponse>.ExternalApiError("Finnhub did not return valid quote data.");
        }

        stock.CurrentPrice = quote.CurrentPrice;
        stock.PreviousClosePrice = quote.PreviousClosePrice;
        stock.ChangePercent = quote.ChangePercent;
        stock.LastUpdatedAt = DateTime.UtcNow;

        await _stockRepository.AddPriceHistoryAsync(new StockPriceHistory
        {
            StockId = stock.Id,
            Price = quote.CurrentPrice,
            RecordedAt = DateTime.UtcNow
        });

        await _stockRepository.SaveChangesAsync();

        return ServiceResult<StockResponse>.Success(ToResponse(stock));
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

    private static string NormalizeSymbol(string? symbol)
    {
        return symbol?.Trim().ToUpperInvariant() ?? string.Empty;
    }

    private static bool IsValidSymbol(string symbol)
    {
        return symbol.Length is > 0 and <= 10
            && symbol.All(character => char.IsLetterOrDigit(character) || character is '.' or '-');
    }
}
