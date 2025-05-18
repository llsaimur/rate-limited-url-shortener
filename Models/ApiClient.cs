namespace UrlShortener.Models;

public class ApiClient
{
    public int Id { get; set; } // EF Core primary key
    public string ApiKey { get; set; }
    public string Name { get; set; }
    public int RateLimit { get; set; } = 10;
}
