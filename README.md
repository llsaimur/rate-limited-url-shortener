# 🔗 Rate-Limited URL Shortener (Full Stack)

A full-stack URL shortening service with **ASP.NET Core** backend and **Angular** frontend. Includes **Redis-backed rate limiting**, **API key authentication**, and **PostgreSQL** for persistence.

---

## ✅ Features

- 🔐 API Key authentication per client
- ⚖️ Token Bucket rate limiting using Redis
- 🔗 URL shortening and redirection
- 🧠 PostgreSQL for data persistence
- 🌐 Angular-based frontend UI
- 🐳 Docker support for PostgreSQL + Redis
- 🧱 Clean architecture using services and middleware

---

## 🚀 Tech Stack

| Layer         | Technology                        |
|---------------|------------------------------------|
| Backend       | ASP.NET Core 6, Entity Framework   |
| Database      | PostgreSQL (via Docker)            |
| Cache         | Redis (via Docker)                 |
| Frontend      | Angular                            |
| Rate Limiting | Token Bucket (Redis-based)         |

---

## 📂 Project Overview

```
root/
├── backend/                # ASP.NET Core API
│   ├── Controllers/
│   ├── Services/
│   ├── Middleware/
│   ├── Data/
│   ├── Models/
│   ├── Program.cs
│   └── appsettings.json
├── frontend/               # Angular App
│   ├── src/
│   └── angular.json
├── docker-compose.yml
└── .env
```

---

## ⚙️ Setup Instructions

### 🔧 Prerequisites
- [.NET SDK 6+](https://dotnet.microsoft.com/download)
- [Node.js + npm](https://nodejs.org/)
- [Angular CLI](https://angular.io/cli) — `npm install -g @angular/cli`
- [Docker](https://www.docker.com/)

---

### 🔑 1. Configure Environment

Create a `.env` file in the root with:

```
ADMIN_API_KEY=supersecretadminkey
DB_CONNECTION=Host=localhost;Port=5432;Database=urls;Username=devuser;Password=devpass
REDIS_CONNECTION=localhost:6379
```

---

### 🐳 2. Start PostgreSQL & Redis

From the root folder:

```bash
docker-compose up -d
```

---

### 🖥️ 3. Run the Backend

```bash
cd backend
dotnet restore
dotnet run
```

Backend starts at: `http://localhost:5002`

---

### 🌐 4. Run the Frontend

```bash
cd frontend
npm install
ng serve
```

Frontend starts at: `http://localhost:4200`

---

## 🔌 API Usage

### 🔐 Header Example
```http
X-Api-Key: your-api-key
```

### 🔗 Key Endpoints

| Method | Endpoint               | Description                          |
|--------|------------------------|--------------------------------------|
| POST   | `/url/shorten`         | Shorten a long URL (requires API key)|
| GET    | `/url/{shortCode}`     | Redirects to the original long URL    |

---

## 📉 Rate Limiting

Implemented via a **Token Bucket algorithm** using Redis:

- Each client has a rate limit (tokens/sec)
- Requests cost tokens
- Redis + Lua scripting ensures atomicity and speed

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

## 🧪 Usage Instructions

### From the UI (Angular)
1. Visit `http://localhost:4200`
2. Paste a URL to shorten
3. Provide your API key when prompted
4. Get a short URL with usage feedback

### From a Tool (e.g. Postman)
```http
POST /url/shorten HTTP/1.1
Host: localhost:5002
Content-Type: application/json
X-Api-Key: your-api-key

{
  "originalUrl": "https://example.com"
}
```

---

## 🛠 Development Notes

- Admin API key is set via `.env`
- CORS is enabled for `localhost:4200`
- Database seeding runs at startup
- Redis + EF Core are DI-injected

---

## 🧪 License

This project is licensed under the [MIT License](LICENSE).

---

## ✨ Credits

Built by Mohammad Saimur Rashid
