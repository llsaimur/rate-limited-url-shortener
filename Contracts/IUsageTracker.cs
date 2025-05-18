namespace UrlShortener.Services;

public interface IUsageTracker
{
    void IncrementRequest(string apiKey);
    void IncrementBlocked(string apiKey);
}