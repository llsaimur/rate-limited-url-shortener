using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using UrlShortener.Contracts;

namespace UrlShortener.Services
{
    public class RedisRateLimiter : IRateLimiter
    {
        private readonly IDatabase _redisDb;
        private readonly int _limit;
        private readonly TimeSpan _window;

        public RedisRateLimiter(IConnectionMultiplexer redis, int limit, TimeSpan window)
        {
            _redisDb = redis.GetDatabase();
            _limit = limit;
            _window = window;
        }

        public bool IsRequestAllowed(string clientId)
        {
            var windowStart = DateTime.UtcNow.ToString("yyyyMMddHHmm"); // per-minute window
            var key = $"rate_limit:{clientId}:{windowStart}";

            var count = _redisDb.StringIncrement(key);
            if (count == 1)
            {
                _redisDb.KeyExpire(key, _window); // set TTL only on first use
            }

            return count <= _limit;
        }
    }
}
