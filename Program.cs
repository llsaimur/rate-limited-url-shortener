using Microsoft.EntityFrameworkCore;
using UrlShortener.Contracts;
using UrlShortener.Data;
using UrlShortener.Middleware;
using UrlShortener.Services;
using StackExchange.Redis;
using UrlShortener.Models;

DotNetEnv.Env.Load();
var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register DB
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register Redis connection
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
    ConnectionMultiplexer.Connect("localhost:6379")); // Or use env/config

builder.Services.AddSingleton<IRateLimiter>(sp =>
{
    var redis = sp.GetRequiredService<IConnectionMultiplexer>();
    return new RedisTokenBucketRateLimiter(redis, refillRate: 0.2);
});

builder.Services.AddSingleton<IUsageTracker, RedisUsageTracker>();


// Register Controllers
builder.Services.AddControllers();

builder.Services.Configure<AdminOptions>(options =>
{
    options.Key = Environment.GetEnvironmentVariable("ADMIN_API_KEY");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Apply Rate Limiting Middleware
app.UseMiddleware<RateLimitingMiddleware>();

// Enable attribute-based routing
app.MapControllers();
app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
