using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Data;
using UrlShortener.Models;

namespace UrlShortener.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UrlController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UrlController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("shorten")]
        public async Task<IActionResult> Shorten([FromBody] UrlRequest request)
        {
            var shortCode = Guid.NewGuid().ToString().Substring(0, 6);
            var mapping = new UrlMapping
            {
                OriginalUrl = request.OriginalUrl,
                ShortCode = shortCode
            };

            _context.UrlMappings.Add(mapping);
            await _context.SaveChangesAsync();

            var baseUrl = $"{Request.Scheme}://{Request.Host}/url/{shortCode}";
            return Ok(new UrlResponse { ShortUrl = baseUrl });
        }

        [HttpGet("{code}")]
        public async Task<IActionResult> RedirectToLong(string code)
        {
            var mapping = await _context.UrlMappings
                .FirstOrDefaultAsync(m => m.ShortCode == code);

            if (mapping == null) return NotFound();

            return Redirect(mapping.OriginalUrl);
        }
    }
}
