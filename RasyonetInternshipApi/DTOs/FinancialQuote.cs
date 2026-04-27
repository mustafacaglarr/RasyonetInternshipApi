namespace RasyonetInternshipApi.DTOs;

public class FinancialQuote
{
    public decimal CurrentPrice { get; set; }

    public decimal PreviousClosePrice { get; set; }

    public decimal ChangePercent { get; set; }
}
