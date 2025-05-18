using Microsoft.EntityFrameworkCore;
using UrlShortener.Contracts;
using UrlShortener.Data;
using UrlShortener.Middleware;
using UrlShortener.Services;
using StackExchange.Redis;
using UrlShortener.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register DB
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

//// Register Rate limiter
//builder.Services.AddSingleton<IRateLimiter>(new TokenBucketRateLimiter(10, 0.2)); // 10 tokens, refills 1 every 5 sec

// Register Redis connection
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect("localhost:6379")); // Or use env/config

//// Use Redis-based rate limiter
//builder.Services.AddSingleton<IRateLimiter>(sp =>
//{
//    var redis = sp.GetRequiredService<IConnectionMultiplexer>();
//    return new RedisRateLimiter(redis, limit: 10, window: TimeSpan.FromMinutes(1));
//});

builder.Services.AddSingleton<IRateLimiter>(sp =>
{
    var redis = sp.GetRequiredService<IConnectionMultiplexer>();
    return new RedisTokenBucketRateLimiter(redis, maxTokens: 10, refillRate: 0.2); // 1 token per 5 seconds
});



// Register Controllers
builder.Services.AddControllers();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    if (!db.ApiClients.Any())
    {
        db.ApiClients.AddRange(
            new ApiClient { ApiKey = "key123", Name = "Alice", RateLimit = 10 },
            new ApiClient { ApiKey = "key456", Name = "Bob", RateLimit = 20 }
        );
        db.SaveChanges();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();



var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

// Apply Rate Limiting Middleware
app.UseMiddleware<RateLimitingMiddleware>();

// Enable attribute-based routing
app.MapControllers();
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
