namespace RasyonetInternshipApi.DTOs;

public class StockResponse
{
    public int Id { get; set; }

    public string Symbol { get; set; } = string.Empty;

    public string CompanyName { get; set; } = string.Empty;

    public decimal CurrentPrice { get; set; }

    public decimal PreviousClosePrice { get; set; }

    public decimal ChangePercent { get; set; }

    public DateTime LastUpdatedAt { get; set; }
}
