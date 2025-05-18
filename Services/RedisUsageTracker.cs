using StackExchange.Redis;

namespace UrlShortener.Services;

public class RedisUsageTracker : IUsageTracker
{
    private readonly IDatabase _db;

    public RedisUsageTracker(IConnectionMultiplexer redis)
    {
        _db = redis.GetDatabase();
    }

    public void IncrementRequest(string apiKey)
    {
        _db.StringIncrement($"stats:requests:{apiKey}");
    }

    public void IncrementBlocked(string apiKey)
    {
        _db.StringIncrement($"stats:blocked:{apiKey}");
    }
}
