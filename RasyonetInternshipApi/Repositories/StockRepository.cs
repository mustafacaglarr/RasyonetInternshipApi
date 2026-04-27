using Microsoft.EntityFrameworkCore;
using RasyonetInternshipApi.Data;
using RasyonetInternshipApi.Models;

namespace RasyonetInternshipApi.Repositories;

public class StockRepository : IStockRepository
{
    private readonly AppDbContext _context;

    public StockRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<List<Stock>> GetAllAsync()
    {
        // Repository Pattern: data access logic is separated from business logic.
        return _context.Stocks
            .OrderBy(stock => stock.Symbol)
            .ToListAsync();
    }

    public Task<Stock?> GetBySymbolAsync(string symbol)
    {
        return _context.Stocks
            .FirstOrDefaultAsync(stock => stock.Symbol == symbol);
    }

    public Task<bool> ExistsAsync(string symbol)
    {
        return _context.Stocks.AnyAsync(stock => stock.Symbol == symbol);
    }

    public async Task AddAsync(Stock stock)
    {
        await _context.Stocks.AddAsync(stock);
    }

    public Task DeleteAsync(Stock stock)
    {
        _context.Stocks.Remove(stock);
        return Task.CompletedTask;
    }

    public async Task AddPriceHistoryAsync(StockPriceHistory priceHistory)
    {
        await _context.StockPriceHistories.AddAsync(priceHistory);
    }

    public Task<List<Stock>> GetTopGainersAsync(int count)
    {
        return _context.Stocks
            .OrderByDescending(stock => stock.ChangePercent)
            .ThenBy(stock => stock.Symbol)
            .Take(count)
            .ToListAsync();
    }

    public Task SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
}
