using UrlShortener.Contracts;

namespace UrlShortener.Services
{
    public class TokenBucketRateLimiter : IRateLimiter
    {
        private readonly int _capacity;
        private readonly double _refillRate;
        private readonly Dictionary<string, (double Tokens, DateTime LastRefill)> _buckets = new();

        public TokenBucketRateLimiter(int capacity, double refillRate)
        {
            _capacity = capacity;
            _refillRate = refillRate;
        }

        public bool IsRequestAllowed(string clientId)
        {
            var now = DateTime.UtcNow;
            if (!_buckets.TryGetValue(clientId, out var bucket))
            {
                _buckets[clientId] = (Tokens: _capacity - 1, LastRefill: now);
                return true;
            }

            var timePassed = (now - bucket.LastRefill).TotalSeconds;
            var newTokens = Math.Min(_capacity, bucket.Tokens + timePassed * _refillRate);

            if (newTokens >= 1)
            {
                _buckets[clientId] = (newTokens - 1, now);
                return true;
            }

            return false;
        }
    }

}
