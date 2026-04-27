using RasyonetInternshipApi.DTOs;

namespace RasyonetInternshipApi.Services;

public interface IStockService
{
    Task<List<StockResponse>> GetAllAsync();

    Task<ServiceResult<StockResponse>> GetBySymbolAsync(string symbol);

    Task<ServiceResult<StockResponse>> AddAsync(CreateStockRequest request);

    Task<ServiceResult<bool>> DeleteAsync(string symbol);

    Task<ServiceResult<StockResponse>> RefreshAsync(string symbol);
}
