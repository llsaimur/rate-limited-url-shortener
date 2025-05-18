using UrlShortener.Contracts;
using UrlShortener.Data;
using UrlShortener.Services;

namespace UrlShortener.Middleware
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;

        public RateLimitingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IRateLimiter limiter, AppDbContext db, IUsageTracker usageTracker)
        {
            if (context.Request.Path.StartsWithSegments("/admin"))
            {
                await _next(context); // skip rate limiting
                return;
            }

            if (!context.Request.Headers.TryGetValue("X-Api-Key", out var apiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Missing API Key");
                return;
            }

            var client = db.ApiClients.FirstOrDefault(c => c.ApiKey == apiKey.ToString());
            if (client == null)
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Invalid API Key");
                return;
            }

            usageTracker.IncrementRequest(client.ApiKey); // Track total requests

            if (!limiter.IsRequestAllowed(client.ApiKey, client.RateLimit))
            {
                usageTracker.IncrementBlocked(client.ApiKey);
                Console.WriteLine($"Client: {client.Name}, API Key: {client.ApiKey}, Limit: {client.RateLimit}");
                context.Response.StatusCode = 429;
                await context.Response.WriteAsync("Rate limit exceeded");
                return;
            }
            // Continue to the next middleware
            await _next(context);
        }
    }
}
