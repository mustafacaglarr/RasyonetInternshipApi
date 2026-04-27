using RasyonetInternshipApi.DTOs;

namespace RasyonetInternshipApi.Services;

public interface IAnalyticsService
{
    Task<ServiceResult<List<StockResponse>>> GetTopGainersAsync(int count);
}
