using UrlShortener.Contracts;
using UrlShortener.Data;

namespace UrlShortener.Middleware
{
    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;

        public RateLimitingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IRateLimiter limiter, AppDbContext db)
        {
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

            if (!limiter.IsRequestAllowed(client.ApiKey))
            {
                context.Response.StatusCode = 429;
                await context.Response.WriteAsync("Rate limit exceeded");
                return;
            }

            // Continue to the next middleware
            await _next(context);
        }
    }


}
