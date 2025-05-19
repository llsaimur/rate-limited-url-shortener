using UrlShortener.Data;
using UrlShortener.Models;

namespace UrlShortener.Helpers
{
    public static class DatabaseHelper
    {
        public static void SeedInitialData(AppDbContext db)
        {
            if (db.ApiClients.Any())
            {
                return;
            }

            var clients = new List<ApiClient>
            {
                new ApiClient {
                    Name = "Alice",
                    ApiKey = "key123",
                    RateLimit = 10
                },
                new ApiClient {
                    Name = "Bob",
                    ApiKey = "key456",
                    RateLimit = 5
                }
            };

            db.ApiClients.AddRange(clients);
            db.SaveChanges();
        }
    }
}
