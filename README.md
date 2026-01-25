# WalletCore

A distributed wallet management system with multi-currency support, built on .NET 9.0 microservices architecture.

## Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Technology Stack](#technology-stack)
- [Project Structure](#project-structure)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [API Reference](#api-reference)
- [Message Contracts](#message-contracts)
- [Database Schema](#database-schema)
- [Development Guide](#development-guide)
- [Docker Deployment](#docker-deployment)

---

## Overview

WalletCore is a distributed wallet management system that enables:

- **Wallet Management**: Create digital wallets in specific currencies
- **Balance Operations**: View balances with real-time currency conversion
- **Balance Adjustments**: Modify wallet balances using configurable strategies
- **Currency Exchange**: Automatic conversion using European Central Bank (ECB) rates
- **Rate Management**: Automatic fetching and caching of exchange rates
- **Monitoring**: Comprehensive structured logging with Elasticsearch integration

---

## Architecture

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                              CLIENT REQUEST                                  │
└─────────────────────────────────────────────────────────────────────────────┘
                                      │
                                      ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                         WALLETCORE API (Port 5106)                          │
│  ┌──────────────────┐  ┌──────────────────┐  ┌──────────────────────────┐  │
│  │ TransactionId    │  │ WalletController │  │ ExceptionHandler         │  │
│  │ Middleware       │  │                  │  │ Middleware               │  │
│  └──────────────────┘  └──────────────────┘  └──────────────────────────┘  │
│                                      │                                      │
│  ┌──────────────────────────────────────────────────────────────────────┐  │
│  │                        APPLICATION LAYER                              │  │
│  │  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐  │  │
│  │  │ Wallet      │  │ ECB         │  │ Rate        │  │ Strategy    │  │  │
│  │  │ Service     │  │ Service     │  │ Converter   │  │ Factory     │  │  │
│  │  └─────────────┘  └─────────────┘  └─────────────┘  └─────────────┘  │  │
│  │  ┌───────────────────────────────────────────────────────────────┐   │  │
│  │  │              ExchangeRateBackgroundJob (1 min)                │   │  │
│  │  └───────────────────────────────────────────────────────────────┘   │  │
│  └──────────────────────────────────────────────────────────────────────┘  │
│                                      │                                      │
│  ┌──────────────────────────────────────────────────────────────────────┐  │
│  │                      INFRASTRUCTURE LAYER                             │  │
│  │  ┌─────────────┐  ┌─────────────┐  ┌─────────────────────────────┐   │  │
│  │  │ ECB HTTP    │  │ DataService │  │ RabbitMQ Command Publisher  │   │  │
│  │  │ Client      │  │ HTTP Client │  │ (MassTransit)               │   │  │
│  │  └─────────────┘  └─────────────┘  └─────────────────────────────┘   │  │
│  └──────────────────────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────────────────────┘
       │                         │                              │
       │ HTTP                    │ HTTP                         │ AMQP
       ▼                         ▼                              │
┌──────────────┐    ┌──────────────────────────────────────┐    │
│   ECB API    │    │   WALLETCORE.DATASERVICE (Port 5000) │    │
│  (External)  │    │  ┌────────────┐  ┌────────────────┐  │    │
└──────────────┘    │  │ Minimal API│  │ Message Consumer│ │◄───┘
                    │  │ Endpoints  │  │ (ExchangeRates) │  │
                    │  └────────────┘  └────────────────┘  │
                    │  ┌────────────────────────────────┐  │
                    │  │         INFRASTRUCTURE         │  │
                    │  │  ┌────────────┐ ┌───────────┐  │  │
                    │  │  │Repositories│ │ DbContext │  │  │
                    │  │  └────────────┘ └───────────┘  │  │
                    │  └────────────────────────────────┘  │
                    └──────────────────────────────────────┘
                                      │
                                      ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│                         INFRASTRUCTURE SERVICES                             │
│                                                                             │
│  ┌─────────────────┐  ┌─────────────────┐  ┌─────────────────┐             │
│  │   SQL Server    │  │     Redis       │  │    RabbitMQ     │             │
│  │   (Database)    │  │    (Cache)      │  │   (Messaging)   │             │
│  │   Port: 1433    │  │   Port: 6379    │  │  Port: 5672     │             │
│  └─────────────────┘  └─────────────────┘  └─────────────────┘             │
│                                                                             │
│  ┌─────────────────┐  ┌─────────────────┐                                  │
│  │  Elasticsearch  │  │     Kibana      │                                  │
│  │    (Logging)    │  │  (Monitoring)   │                                  │
│  │   Port: 9200    │  │   Port: 5601    │                                  │
│  └─────────────────┘  └─────────────────┘                                  │
└─────────────────────────────────────────────────────────────────────────────┘
```

### Design Patterns

| Pattern | Implementation | Purpose |
|---------|---------------|---------|
| **Strategy** | `IWalletBalanceStrategy` | Different balance adjustment algorithms |
| **Decorator** | `CachedEcbService` wraps `EcbService` | Adds caching layer |
| **Factory** | `IWalletBalanceStrategyFactory` | Creates balance strategies |
| **Repository** | `IWalletRepository`, `IExchangeRateMergeRepository` | Data access abstraction |
| **Pub/Sub** | MassTransit with RabbitMQ | Asynchronous exchange rate updates |

---

## Technology Stack

| Layer | Technology | Version |
|-------|------------|---------|
| **Runtime** | .NET | 9.0 |
| **Language** | C# | 12 |
| **API Framework** | ASP.NET Core | 9.0 |
| **ORM** | Entity Framework Core | 9.0.11 |
| **Messaging** | MassTransit | 8.5.7 |
| **Message Broker** | RabbitMQ | 3 |
| **Caching** | Redis (StackExchange.Redis) | 7 / 2.10.1 |
| **Logging** | Serilog | 8.0+ |
| **Log Storage** | Elasticsearch | 8.14.1 |
| **Log Visualization** | Kibana | 8.14.1 |
| **Resilience** | Polly | 8.6.5 |
| **DI Extensions** | Scrutor | 7.0.0 |
| **Database** | SQL Server | - |

---

## Project Structure

```
C:\DevRepo
│
├── WalletCore/                         # Main API Solution
│   ├── WalletCore/                     # Web API Project
│   │   ├── Controllers/
│   │   │   └── WalletController.cs     # REST API endpoints
│   │   ├── Middleware/
│   │   │   ├── TransactionIdMiddleware.cs
│   │   │   └── ExceptionHandler.cs
│   │   ├── appsettings.json
│   │   └── Program.cs                  # Entry point
│   │
│   ├── WalletCore.Application/         # Business Logic Layer
│   │   ├── Services/
│   │   │   ├── WalletService.cs        # Core wallet operations
│   │   │   ├── EcbService.cs           # ECB rate fetching
│   │   │   ├── CachedEcbService.cs     # Cached rate access
│   │   │   └── EcbRateConverter.cs     # Currency conversion
│   │   ├── Strategies/
│   │   │   └── WalletBalanceStrategyFactory.cs
│   │   ├── BackgroundJobs/
│   │   │   └── ExchangeRateBackgroundJob.cs
│   │   └── ApplicationModule.cs        # DI registration
│   │
│   ├── WalletCore.Infrastructure/      # External Service Integration
│   │   ├── HttpClients/
│   │   │   ├── ECBHttpClient.cs
│   │   │   └── WalletDataServiceHttpClient.cs
│   │   └── CommandPublisher.cs         # RabbitMQ publisher
│   │
│   ├── WalletCore.Domain/              # Domain Models
│   │   ├── DBModels/
│   │   ├── RequestModels/
│   │   ├── ResponseModels/
│   │   └── Exceptions/
│   │
│   ├── WalletCore.Logging/             # Logging Library
│   │   └── LoggerExtensions.cs
│   │
│   ├── docker-compose.yaml             # Infrastructure services
│   └── WalletCore.sln
│
├── WalletCore.Contracts/               # Shared Contracts (NuGet Package)
│   └── WalletCore.Contracts/
│       ├── CommandContracts/
│       │   ├── CreateWalletCommand.cs
│       │   └── MergeExchangeRatesCommand.cs
│       ├── DBModels/
│       │   ├── Wallet.cs
│       │   └── ExchangeRate.cs
│       ├── RequestModels/
│       └── ResponseModels/
│
├── WalletCore.DataService/             # Data Access Solution
│   ├── WalletCore.DataService/         # Worker Service / Minimal API
│   │   ├── appsettings.json
│   │   └── Program.cs
│   │
│   ├── WalletCore.DataService.Infrastructure/
│   │   ├── WalletDbContext.cs          # EF Core context
│   │   ├── Repositories/
│   │   │   ├── WalletRepository.cs
│   │   │   └── ExchangeRateMergeRepository.cs
│   │   ├── Consumers/
│   │   │   └── MergeExchangeRatesConsumer.cs
│   │   └── CacheService.cs
│   │
│   └── WalletCore.DataService.sln
│
└── WalletCore.Logging/                 # Standalone Logging Package
    └── WalletCore.Logging/
        └── LoggerExtensions.cs
```

---

## Prerequisites

- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (for infrastructure services)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) (LocalDB or full instance)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

---

## Getting Started

### 1. Clone the Repository

```bash
git clone <repository-url>
cd DevRepo
```

### 2. Start Infrastructure Services

```bash
cd WalletCore
docker-compose up -d
```

This starts:
- **Redis** on port `6379`
- **RabbitMQ** on ports `5672` (AMQP) and `15672` (Management UI)
- **Elasticsearch** on port `9200`
- **Kibana** on port `5601`

### 3. Configure the Database

Update the connection string in both `appsettings.json` files:

**WalletCore/WalletCore/appsettings.json:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=ExchangeRateDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

**WalletCore.DataService/WalletCore.DataService/appsettings.json:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=ExchangeRateDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### 4. Apply Database Migrations

```bash
cd WalletCore.DataService/WalletCore.DataService.Infrastructure
dotnet ef database update --startup-project ../WalletCore.DataService
```

### 5. Build and Run the Services

**Terminal 1 - Start DataService:**
```bash
cd WalletCore.DataService
dotnet run --project WalletCore.DataService
```

**Terminal 2 - Start WalletCore API:**
```bash
cd WalletCore
dotnet run --project WalletCore
```

### 6. Verify the Services

- **WalletCore API**: http://localhost:5106
- **DataService API**: http://localhost:5000
- **RabbitMQ Management**: http://localhost:15672 (guest/guest)
- **Kibana**: http://localhost:5601

---

## Configuration

### WalletCore API Configuration

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=ExchangeRateDB;..."
  },
  "ECBConfig": {
    "BaseUrl": "https://www.ecb.europa.eu",
    "DailyRatesEndpoint": "/stats/eurofxref/eurofxref-daily.xml"
  },
  "WalletDataServiceConfig": {
    "BaseUrl": "http://localhost:5000"
  },
  "Redis": {
    "Configuration": "localhost:6379"
  },
  "Serilog": {
    "WriteTo": [
      { "Name": "Console" },
      {
        "Name": "Elasticsearch",
        "Args": {
          "nodeUris": "http://localhost:9200",
          "indexFormat": "walletcore-logs-{0:yyyy.MM.dd}"
        }
      }
    ]
  }
}
```

### Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `ASPNETCORE_ENVIRONMENT` | Runtime environment | `Development` |
| `ConnectionStrings__DefaultConnection` | SQL Server connection | - |
| `Redis__Configuration` | Redis connection string | `localhost:6379` |

---

## API Reference

### Base URL

```
http://localhost:5106/api/wallet
```

### Headers

| Header | Required | Description |
|--------|----------|-------------|
| `Content-Type` | Yes | `application/json` |
| `X-Transaction-Id` | No | Unique request identifier for tracing |

---

### Create Wallet

Creates a new wallet with the specified currency.

**Endpoint:** `POST /api/wallet/createWallet`

**Request:**
```json
{
  "currency": "USD"
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "walletId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "currency": "USD",
    "balance": 0.00000000
  },
  "error": null
}
```

**Response Codes:**
| Code | Description |
|------|-------------|
| 200 | Wallet created successfully |
| 400 | Invalid currency code |
| 500 | Internal server error |

---

### Get Balance

Retrieves the wallet balance, optionally converted to a different currency.

**Endpoint:** `POST /api/wallet/getBalance`

**Request:**
```json
{
  "walletId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "targetCurrency": "EUR"
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `walletId` | GUID | Yes | The wallet identifier |
| `targetCurrency` | string | No | Currency to convert balance to (ISO 4217) |

**Response:**
```json
{
  "success": true,
  "data": {
    "walletId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "balance": 100.00000000,
    "currency": "EUR",
    "originalCurrency": "USD",
    "originalBalance": 108.50000000,
    "conversionRate": 0.92165899
  },
  "error": null
}
```

**Response Codes:**
| Code | Description |
|------|-------------|
| 200 | Balance retrieved successfully |
| 404 | Wallet not found |
| 400 | Invalid currency code |
| 500 | Internal server error |

---

### Adjust Balance

Adjusts the wallet balance using a specified strategy.

**Endpoint:** `POST /api/wallet/adjustBalance`

**Request:**
```json
{
  "walletId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "amount": 50.00,
  "currency": "EUR",
  "operation": "Add"
}
```

| Field | Type | Required | Description |
|-------|------|----------|-------------|
| `walletId` | GUID | Yes | The wallet identifier |
| `amount` | decimal | Yes | Amount to adjust |
| `currency` | string | Yes | Currency of the amount (ISO 4217) |
| `operation` | string | Yes | Operation type: `Add` or `Subtract` |

**Response:**
```json
{
  "success": true,
  "data": {
    "walletId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "previousBalance": 100.00000000,
    "newBalance": 154.25000000,
    "currency": "USD",
    "adjustmentAmount": 54.25000000,
    "originalAmount": 50.00,
    "originalCurrency": "EUR"
  },
  "error": null
}
```

**Response Codes:**
| Code | Description |
|------|-------------|
| 200 | Balance adjusted successfully |
| 404 | Wallet not found |
| 400 | Invalid operation or insufficient funds |
| 500 | Internal server error |

---

## Message Contracts

The system uses RabbitMQ for asynchronous communication for background operations (exchange rate updates). Wallet operations (create, get balance, adjust balance) are synchronous HTTP calls.

### MergeExchangeRatesCommand

Published by the background job when fetching new exchange rates from the ECB.

```csharp
public record MergeExchangeRatesCommand
{
    public List<ExchangeRate> ExchangeRates { get; init; }
}
```

---

## Database Schema

### Wallets Table

| Column | Type | Description |
|--------|------|-------------|
| `Id` | `UNIQUEIDENTIFIER` | Primary key |
| `Balance` | `DECIMAL(18,8)` | Wallet balance with high precision |
| `Currency` | `NVARCHAR(3)` | ISO 4217 currency code |

**Indexes:**
- Primary key on `Id`
- Index on `Currency`

### ExchangeRates Table

| Column | Type | Description |
|--------|------|-------------|
| `Id` | `UNIQUEIDENTIFIER` | Primary key |
| `Date` | `DATETIME2` | Rate date |
| `CurrencyCode` | `NVARCHAR(3)` | ISO 4217 currency code |
| `Rate` | `DECIMAL(18,8)` | Exchange rate to EUR |

**Indexes:**
- Primary key on `Id`
- Unique index on `(Date, CurrencyCode)`

---

## Development Guide

### Building the Solution

```bash
# Build WalletCore
cd WalletCore
dotnet build WalletCore.sln

# Build DataService
cd ../WalletCore.DataService
dotnet build WalletCore.DataService.sln
```

### Running Tests

```bash
dotnet test WalletCore.sln
```

### Adding Database Migrations

```bash
cd WalletCore.DataService/WalletCore.DataService.Infrastructure
dotnet ef migrations add MigrationName --startup-project ../WalletCore.DataService
```

### Local Package Development

The solution uses local NuGet packages for shared code:

1. **WalletCore.Contracts** (v2.0.0) - Shared DTOs and commands
2. **WalletCore.Logging** (v1.0.0) - Logging extensions

To update packages after changes:

```bash
# Pack the contracts
cd WalletCore.Contracts/WalletCore.Contracts
dotnet pack -c Release

# Pack the logging
cd ../../WalletCore.Logging/WalletCore.Logging
dotnet pack -c Release
```

### Code Style

- Follow Microsoft C# coding conventions
- Use async/await for I/O operations
- Apply SOLID principles
- Use dependency injection for all services

---

## Docker Deployment

### Infrastructure Services

The `docker-compose.yaml` provides all required infrastructure:

```yaml
services:
  redis:
    image: redis:7
    ports:
      - "6379:6379"

  elasticsearch:
    image: docker.elastic.co/elasticsearch/elasticsearch:8.14.1
    ports:
      - "9200:9200"
    environment:
      - discovery.type=single-node
      - xpack.security.enabled=false

  kibana:
    image: docker.elastic.co/kibana/kibana:8.14.1
    ports:
      - "5601:5601"
    depends_on:
      - elasticsearch

  rabbitmq:
    image: rabbitmq:3-management
    ports:
      - "5672:5672"
      - "15672:15672"
```

### Start Infrastructure

```bash
cd WalletCore
docker-compose up -d
```

### Stop Infrastructure

```bash
docker-compose down
```

### View Logs

```bash
docker-compose logs -f
```

---

## Monitoring

### Logging

All services log to:
- **Console** - Development debugging
- **Elasticsearch** - Centralized log storage

Log index pattern: `walletcore-logs-{yyyy.MM.dd}`

### Kibana Dashboard

Access Kibana at http://localhost:5601 to:
- View application logs
- Create visualizations
- Set up alerts
- Trace requests using Transaction ID

### Transaction Tracing

Include the `X-Transaction-Id` header in requests to trace operations across services:

```bash
curl -X POST http://localhost:5106/api/wallet/getBalance \
  -H "Content-Type: application/json" \
  -H "X-Transaction-Id: my-trace-id-123" \
  -d '{"walletId": "..."}'
```

---

## Troubleshooting

### Common Issues

**RabbitMQ connection refused:**
```
Ensure RabbitMQ is running: docker-compose up -d rabbitmq
Check port 5672 is not in use
```

**Redis connection failed:**
```
Ensure Redis is running: docker-compose up -d redis
Verify Redis configuration in appsettings.json
```

**Database connection failed:**
```
Verify SQL Server is running
Check connection string in appsettings.json
Ensure database exists and migrations are applied
```

**Exchange rates not updating:**
```
Check ExchangeRateBackgroundJob logs
Verify ECB API is accessible
Ensure RabbitMQ consumers are running
```

---

## License

[Add license information here]

---

## Contributing

[Add contribution guidelines here]
