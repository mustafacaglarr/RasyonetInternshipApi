namespace RasyonetInternshipApi.Models;

public class StockPriceHistory
{
    public int Id { get; set; }

    public int StockId { get; set; }

    public decimal Price { get; set; }

    public DateTime RecordedAt { get; set; }

    public Stock? Stock { get; set; }
}
