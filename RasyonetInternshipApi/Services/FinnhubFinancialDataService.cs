using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using RasyonetInternshipApi.Configuration;
using RasyonetInternshipApi.DTOs;

namespace RasyonetInternshipApi.Services;

public class FinnhubFinancialDataService : IFinancialDataService
{
    private readonly HttpClient _httpClient;
    private readonly FinnhubOptions _options;

    public FinnhubFinancialDataService(HttpClient httpClient, IOptions<FinnhubOptions> options)
    {
        _httpClient = httpClient;
        _options = options.Value;
    }

    public async Task<FinancialQuote?> GetQuoteAsync(string symbol)
    {
        if (string.IsNullOrWhiteSpace(_options.ApiKey))
        {
            throw new InvalidOperationException("Finnhub API key is not configured.");
        }

        var requestUri = $"{_options.BaseUrl.TrimEnd('/')}/quote?symbol={Uri.EscapeDataString(symbol)}&token={Uri.EscapeDataString(_options.ApiKey)}";
        using var response = await _httpClient.GetAsync(requestUri);

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        await using var stream = await response.Content.ReadAsStreamAsync();
        var quote = await JsonSerializer.DeserializeAsync<FinnhubQuoteResponse>(
            stream,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (quote is null || quote.CurrentPrice <= 0)
        {
            return null;
        }

        return new FinancialQuote
        {
            CurrentPrice = quote.CurrentPrice,
            PreviousClosePrice = quote.PreviousClosePrice,
            ChangePercent = quote.ChangePercent
        };
    }

    private class FinnhubQuoteResponse
    {
        [JsonPropertyName("c")]
        public decimal CurrentPrice { get; set; }

        [JsonPropertyName("pc")]
        public decimal PreviousClosePrice { get; set; }

        [JsonPropertyName("dp")]
        public decimal ChangePercent { get; set; }
    }
}
