using StackExchange.Redis;
using System;
using UrlShortener.Contracts;
using UrlShortener.Helpers;

namespace UrlShortener.Services;

public class RedisTokenBucketRateLimiter : IRateLimiter
{
    private readonly IDatabase _db;
    private readonly double _refillRate; // tokens per second
    private string _luaScript;

    public RedisTokenBucketRateLimiter(IConnectionMultiplexer redis, double refillRate)
    {        
        _refillRate = refillRate;

        _db = redis.GetDatabase();
        LoadScripts();
    }

    private void LoadScripts()
    {
        string filePath = FileHelper.GetAbsolutePath("Properties/Scripts/TokenBucket.lua");
        _luaScript = FileHelper.LoadFile(filePath);
    }

    public bool IsRequestAllowed(string clientId, int maxTokens)
    {
        var key = $"rate_limit:{clientId}";
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        var redisKey = new RedisKey[]
        {
            key
        };

        var redisValue = new RedisValue[]
        {
            maxTokens,
            _refillRate,
            now
        };

        var result = (int)_db.ScriptEvaluate(_luaScript, redisKey, redisValue);
        Console.WriteLine($"Redis rate limiter result for {clientId}: {result}");


        return result == 1;
    }
}
