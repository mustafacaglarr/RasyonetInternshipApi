using RasyonetInternshipApi.Models;

namespace RasyonetInternshipApi.Repositories;

public interface IStockRepository
{
    Task<List<Stock>> GetAllAsync();

    Task<Stock?> GetBySymbolAsync(string symbol);

    Task<bool> ExistsAsync(string symbol);

    Task AddAsync(Stock stock);

    Task DeleteAsync(Stock stock);

    Task AddPriceHistoryAsync(StockPriceHistory priceHistory);

    Task<List<Stock>> GetTopGainersAsync(int count);

    Task SaveChangesAsync();
}
