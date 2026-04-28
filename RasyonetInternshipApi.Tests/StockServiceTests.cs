using RasyonetInternshipApi.DTOs;
using RasyonetInternshipApi.Models;
using RasyonetInternshipApi.Repositories;
using RasyonetInternshipApi.Services;

namespace RasyonetInternshipApi.Tests;

public class StockServiceTests
{
    [Fact]
    public async Task AddAsync_WhenStockAlreadyExists_ReturnsConflict()
    {
        var repository = new FakeStockRepository(stockExists: true);
        var financialDataService = new FakeFinancialDataService();
        var service = new StockService(repository, financialDataService);

        var result = await service.AddAsync(new CreateStockRequest
        {
            Symbol = "AAPL",
            CompanyName = "Apple Inc."
        });

        Assert.Equal(ServiceResultStatus.Conflict, result.Status);
        Assert.False(repository.AddWasCalled);
        Assert.False(repository.SaveChangesWasCalled);
    }

    private class FakeStockRepository : IStockRepository
    {
        private readonly bool _stockExists;

        public FakeStockRepository(bool stockExists)
        {
            _stockExists = stockExists;
        }

        public bool AddWasCalled { get; private set; }

        public bool SaveChangesWasCalled { get; private set; }

        public Task<List<Stock>> GetAllAsync()
        {
            return Task.FromResult(new List<Stock>());
        }

        public Task<Stock?> GetBySymbolAsync(string symbol)
        {
            return Task.FromResult<Stock?>(null);
        }

        public Task<bool> ExistsAsync(string symbol)
        {
            return Task.FromResult(_stockExists);
        }

        public Task AddAsync(Stock stock)
        {
            AddWasCalled = true;
            return Task.CompletedTask;
        }

        public Task DeleteAsync(Stock stock)
        {
            return Task.CompletedTask;
        }

        public Task AddPriceHistoryAsync(StockPriceHistory priceHistory)
        {
            return Task.CompletedTask;
        }

        public Task<List<Stock>> GetTopGainersAsync(int count)
        {
            return Task.FromResult(new List<Stock>());
        }

        public Task SaveChangesAsync()
        {
            SaveChangesWasCalled = true;
            return Task.CompletedTask;
        }
    }

    private class FakeFinancialDataService : IFinancialDataService
    {
        public Task<FinancialQuote?> GetQuoteAsync(string symbol)
        {
            return Task.FromResult<FinancialQuote?>(null);
        }
    }
}
