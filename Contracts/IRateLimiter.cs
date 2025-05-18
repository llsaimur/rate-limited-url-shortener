namespace UrlShortener.Contracts
{
    public interface IRateLimiter
    {
        bool IsRequestAllowed(string clientId);
    }

}
