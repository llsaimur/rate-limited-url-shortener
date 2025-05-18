using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using UrlShortener.Data;
using UrlShortener.Models;

namespace UrlShortener.Controllers;

[ApiController]
[Route("admin")]
public class AdminController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly AdminOptions _adminOptions;

    public AdminController(AppDbContext db, IOptions<AdminOptions> adminOptions)
    {
        _db = db;
        _adminOptions = adminOptions.Value;
    }

    [HttpPost("clients")]
    public IActionResult CreateClient([FromHeader(Name = "X-Admin-Key")] string adminKey, [FromBody] ApiClientRequest request)
    {
        if (adminKey != _adminOptions.Key)
        {
            return Unauthorized("Invalid admin key.");
        }

        var apiKey = Guid.NewGuid().ToString("N"); // random key without dashes

        var client = new ApiClient
        {
            Name = request.Name,
            ApiKey = apiKey,
            RateLimit = request.RateLimit
        };

        _db.ApiClients.Add(client);
        _db.SaveChanges();

        return Ok(new
        {
            client.Name,
            client.ApiKey,
            client.RateLimit
        });
    }

    [HttpGet("clients")]
    public IActionResult GetClients([FromHeader(Name = "X-Admin-Key")] string adminKey)
    {
        if (adminKey != _adminOptions.Key)
        {
            return Unauthorized("Invalid admin key.");
        }

        var clients = _db.ApiClients
            .Select(c => new
            {
                c.Name,
                c.ApiKey,
                c.RateLimit
            }).ToList();

        return Ok(clients);
    }

    [HttpGet("stats")]
    public IActionResult GetStats([FromHeader(Name = "X-Admin-Key")] string adminKey, [FromServices] IConnectionMultiplexer redis)
    {
        if (adminKey != _adminOptions.Key)
            return Unauthorized("Invalid admin key.");

        var db = redis.GetDatabase();

        var clients = _db.ApiClients.ToList();

        var stats = clients.Select(client => new
        {
            client.Name,
            client.ApiKey,
            client.RateLimit,
            Requests = (int)(db.StringGet($"stats:requests:{client.ApiKey}")),
            Blocked = (int)(db.StringGet($"stats:blocked:{client.ApiKey}"))
        });

        return Ok(stats);
    }
}