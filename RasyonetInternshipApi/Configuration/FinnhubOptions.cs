namespace RasyonetInternshipApi.Configuration;

public class FinnhubOptions
{
    public const string SectionName = "Finnhub";

    public string BaseUrl { get; set; } = "https://finnhub.io/api/v1";

    public string ApiKey { get; set; } = string.Empty;
}
