using StackExchange.Redis;
using System;
using UrlShortener.Contracts;

namespace UrlShortener.Services;

public class RedisTokenBucketRateLimiter : IRateLimiter
{
    private readonly IDatabase _db;
    private readonly int _maxTokens;
    private readonly double _refillRate; // tokens per second
    private readonly string _luaScript;

    public RedisTokenBucketRateLimiter(IConnectionMultiplexer redis, int maxTokens, double refillRate)
    {
        _db = redis.GetDatabase();
        _maxTokens = maxTokens;
        _refillRate = refillRate;

        _luaScript = @"
            local bucket = redis.call('HMGET', KEYS[1], 'tokens', 'last_refill')
            local tokens = tonumber(bucket[1])
            local last_refill = tonumber(bucket[2])

            if tokens == nil then
                tokens = tonumber(ARGV[1])
                last_refill = tonumber(ARGV[3])
            end

            local now = tonumber(ARGV[3])
            local elapsed = now - last_refill
            local refill = elapsed * tonumber(ARGV[2])
            tokens = math.min(tonumber(ARGV[1]), tokens + refill)

            local allowed = 0
            if tokens >= 1 then
                tokens = tokens - 1
                allowed = 1
            end

            redis.call('HMSET', KEYS[1], 'tokens', tokens, 'last_refill', now)
            redis.call('EXPIRE', KEYS[1], 3600)

            return allowed
        ";
    }

    public bool IsRequestAllowed(string clientId)
    {
        var key = $"rate_limit:{clientId}";
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        var result = (int)_db.ScriptEvaluate(_luaScript, new RedisKey[] { key }, new RedisValue[]
        {
            _maxTokens,
            _refillRate,
            now
        });

        return result == 1;
    }
}
