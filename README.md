
# 🔗 Rate-Limited URL Shortener API (Full Stack - WIP)

A modern, full-stack URL shortening service built with **ASP.NET Core** and soon-to-be **Angular** frontend. This project features **distributed rate limiting**, **API key authentication**, and a clean scalable architecture.

---

## ✅ Features Implemented (Backend)

- ✔️ URL shortening and redirection via REST API
- ✔️ Token Bucket **rate limiter** using Redis + Lua scripting
- ✔️ Per-user **API key** authentication
- ✔️ **SQLite database** for persistent URL and user storage
- ✔️ Middleware for rate-limiting and authentication enforcement
- ✔️ Postman-tested endpoints
- ✔️ Clean architecture with services, models, and middleware

---

## 🚀 Tech Stack

| Layer     | Stack                               |
|-----------|--------------------------------------|
| Backend   | ASP.NET Core 6+, Entity Framework    |
| Storage   | SQLite (PostgreSQL planned)          |
| Caching   | Redis (Docker)                       |
| Rate Limiting | Token Bucket (Lua Script)        |
| Frontend  | Angular (planned)                    |
| Testing   | Postman, xUnit (planned)             |

---

## 📦 Project Structure (Backend)

```
UrlShortener/
├── Controllers/              # API endpoints
├── Services/                 # Business logic + rate limiting
├── Middleware/               # Request interception
├── Data/                     # EF Core DbContext
├── Models/                   # API + DB models
├── Program.cs                # Startup config
└── appsettings.json
```

---

## 📫 API Usage

### 🔐 Headers
```
X-Api-Key: key123
```

### 🔗 Endpoints

| Method | URL                     | Description                        |
|--------|--------------------------|------------------------------------|
| POST   | /url/shorten            | Shorten a long URL (requires API key) |
| GET    | /url/{shortCode}        | Redirect to original URL           |

---

## 🧪 Rate Limiting

Using a **Redis-backed Token Bucket algorithm**, with:

- Max tokens: based on user’s `RateLimit`
- Refill rate: configurable (currently fixed per plan)
- TTL and atomicity via Lua scripting

---

## 🧱 Database Models

### `UrlMapping`
```csharp
int Id
string OriginalUrl
string ShortCode
DateTime CreatedAt
```

### `ApiClient`
```csharp
int Id
string ApiKey
string Name
int RateLimit
```

---

## 🛠 Setup Instructions

### Prerequisites
- [.NET SDK 6+](https://dotnet.microsoft.com/download)
- Redis (run via Docker: `docker run -p 6379:6379 redis`)
- SQLite or PostgreSQL

### Run the App

```bash
dotnet restore
dotnet ef database update
dotnet run
```

---

## 🚧 Work in Progress / To-Do

### Backend
- [x] Dynamic rate limits per `ApiClient`
- [x] API key generation & rotation endpoints
- [x] Usage analytics API + dashboard
- [ ] Migrate to PostgreSQL
- [ ] Unit + integration test coverage

### Frontend (Angular)
- [ ] URL input + shorten interface
- [ ] API key login UI
- [ ] Admin dashboard to view usage
- [ ] Error handling and status UI

---

## 🌍 Live Demo / Deployment
> Coming soon — may use Render, Railway, or Azure App Service.

---

## 🧪 License

This project is licensed under the [MIT License](LICENSE).

---

## ✨ Credits

Built by Mohammad Saimur Rashid
