using RasyonetInternshipApi.DTOs;

namespace RasyonetInternshipApi.Services;

public interface IFinancialDataService
{
    Task<FinancialQuote?> GetQuoteAsync(string symbol);
}
