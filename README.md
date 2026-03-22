# Magnise API

A REST API service for retrieving real-time price information for market assets (e.g. EUR/USD, GBP/USD, etc.). Data is sourced from the Fintacharts platform via WebSocket and REST API.

---

## Tech Stack

- .NET 10 / ASP.NET Core
- PostgreSQL 18
- Entity Framework Core
- Docker / Docker Compose
- Fintacharts API (WebSocket + REST)

---

## Architecture
The project follows **Clean Architecture** principles:

- `Magnise.Domain` — Entities and repository interfaces
- `Magnise.Application` — Business logic, services, DTOs
- `Magnise.Infrastructure` — Repository implementations, DB context, Fintacharts clients
- `Magnise.Api` — Controllers, DI registration, entry point

---

## Requirements

- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- Docker Compose (included with Docker Desktop)

---

## Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/Nikolas321654/Magnise_test_work
cd Magnise_test_work/src/Magnise.Api
```

### 2. Run with Docker Compose

```bash
docker-compose up --build
```

This will automatically:
- Build the API Docker image
- Start a PostgreSQL container
- Wait for the database to be ready (healthcheck)
- Apply EF Core migrations
- Seed the list of instruments from Fintacharts
- Connect to the WebSocket for live price updates

### 3. API is available at

```
http://localhost:8080
```

### Stop

```bash
docker-compose down
```

Stop and remove database volume:

```bash
docker-compose down -v
```

---

## Local Development (without Docker)

### 1. Start only the database

```bash
docker-compose up db
```

### 2. Run the application

```bash
dotnet run --project Magnise.Api
```

---

## Configuration

All required configuration is already included in the project files.

`appsettings.json` — used in Docker (Production environment):

```json
{
  "Fintacharts": {
    "BaseUrl": "https://platform.fintacharts.com",
    "WssUrl": "wss://platform.fintacharts.com",
    "Realm": "fintatech",
    "ClientId": "app-cli",
    "Username": "r_test@fintatech.com",
    "Password": "kisfiz-vUnvy9-sopnyv"
  },
  "ConnectionStrings": {
    "AssetDbContext": "Host=db;Port=5432;Database=AssetDb;Username=postgres;Password=magnise"
  }
}
```

`appsettings.Development.json` — used for local development:

```json
{
  "ConnectionStrings": {
    "AssetDbContext": "Host=localhost;Port=5432;Database=AssetDb;Username=postgres;Password=magnise"
  }
}
```

---

## API Endpoints

### GET /api/assets

Returns a list of all supported market assets stored in the database.

**Response example:**

```json
["EURUSD", "GBPUSD", "USDJPY", "NZDUSD"]
```

---

### GET /api/assets/price?symbol={symbol}

Returns the current price and last update time for the specified asset.

**Query parameters:**

| Parameter | Type   | Description         | Example |
|-----------|--------|---------------------|--------|
| symbol    | string | Market asset symbol | EURUSD |

**Response example:**

```json
{
  "symbol": "EUR/USD",
  "price": 1.08231,
  "updatedAt": "2024-01-15T10:30:05Z"
}
```

**Response codes:**

| Code | Description     |
|------|-----------------|
| 200  | Success         |
| 404  | Asset not found |

---

## How It Works

1. On startup, the service authenticates with Fintacharts using OAuth2 password grant
2. Fetches the list of forex instruments via REST API and saves them to the database
3. Connects to the Fintacharts WebSocket and subscribes to all instruments
4. On each price tick, updates the `AssetPrices` table in the database
5. The access token is automatically refreshed before expiry using the refresh token

---

## Project Structure

```
Magnise/
├── Magnise.Api/
│   ├── Controllers/
│   │   └── UserController.cs
│   ├── DTO/
│   ├── appsettings.json
│   ├── appsettings.Development.json
│   ├── Dockerfile
│   └── Program.cs
├── Magnise.Application/
│   ├── DTOs/
│   │   └── AssetPriceResponse.cs
│   ├── Interfaces/
│   │   └── IAssetService.cs
│   └── Services/
│       └── AssetService.cs
├── Magnise.Domain/
│   ├── Entities/
│   │   ├── AssetEntity.cs
│   │   └── AssetPriceEntity.cs
│   └── Interfaces/
│       └── Repositories/
│           └── IAssetRepository.cs
├── Magnise.Infrastructure/
│   ├── Configurations/
│   │   ├── AssetConfiguration.cs
│   │   └── AssetPriceConfiguration.cs
│   ├── Fintacharts/
│   │   ├── DTO/
│   │   ├── FintachartsAuthService.cs
│   │   ├── FintachartsRestClient.cs
│   │   ├── FintachartsWsClient.cs
│   │   └── WsBackgroundService.cs
│   ├── Migrations/
│   ├── Repositories/
│   │   └── AssetRepository.cs
│   └── AssetDbContext.cs
└── compose.yaml
```

---

## Database Schema

```
Assets
──────────────────────────
string (PK): Id           ← Fintacharts instrument ID
string:      Symbol       ← e.g. "EUR/USD"

AssetPrices
──────────────────────────
string (PK, FK): AssetId  ← references Assets.Id
decimal:         Price
datetime:        UpdatedAt
```