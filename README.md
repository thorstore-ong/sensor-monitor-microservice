# Sensor Monitor Microservice

A .NET 10 microservice that ingests sensor telemetry via an ASP.NET Core Web API, processes it through a bounded in-process channel, and persists it to PostgreSQL using Entity Framework Core.

Built as an internship mini project at **IoT.nxt** to understand the data acquisition pipeline — how sensor data from physical equipment flows from a raw JSON payload all the way into a database.

---

## Overview

IoT platforms depend on a continuous stream of telemetry from equipment like generators, meters, and rectifiers. This project simulates that pipeline:

1. A POST request delivers raw JSON sensor data to the API
2. The data is validated — missing required fields throw an error
3. The data is normalised into the correct C# types (e.g. string timestamps → `DateTime`)
4. The normalised record is written to a **bounded `Channel<SensorTelemetry>`** (capacity 100, single reader)
5. A hosted background service (`TelemetryProcessor`) reads from the channel and writes to PostgreSQL

---

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core Web API (.NET 10) |
| Database | PostgreSQL |
| ORM | Entity Framework Core 10 + Npgsql |
| Messaging | `System.Threading.Channels` (in-process) |
| API Docs | Swagger / Swashbuckle |
| IDE | Visual Studio |
| Language | C# |

---

## Architecture

```
POST /api/telemetry  (raw JSON)
        │
        ▼
  [SensorController]
  Orchestrates the pipeline
        │
        ▼
  [TelemetryValidator]
  Checks all required fields — throws if any are missing
        │
        ▼
  [TelemetryNormalizer]  (singleton)
  Converts raw DTO values to typed domain model fields
        │
        ▼
  [Channel<SensorTelemetry>]  (bounded, capacity 100)
  Decouples the HTTP request from the database write
        │
        ▼
  [TelemetryProcessor]  (IHostedService)
  Background service — reads from channel, writes to PostgreSQL
        │
        ▼
  [TelemetryDbContext]  (EF Core + Npgsql)
  PostgreSQL persistence
```

### Key Design Decisions

- **DTOs** hold the raw JSON shape; **domain models** define the storage schema. Transformation happens between the two in the normalizer.
- **Validation runs before normalisation** — bad data is rejected before it touches any conversion logic.
- **`Channel<T>` over a message broker** — a lightweight, in-process alternative suited for single-service pipelines where an external broker (RabbitMQ, Kafka) would be overkill.
- **`SingleReader = true`** on the channel matches the single `TelemetryProcessor` consumer registered as a hosted service.
- **EF Core migrations** are applied automatically at startup via `dbContext.Database.Migrate()`.
- **`TelemetryNormalizer`** is registered as a singleton; **`SensorService`** is scoped.

---

## Project Structure

```
SensorMonitor/
├── Controllers/              # API endpoints — orchestrates validate → normalize → channel write
├── DTOs/                     # Raw JSON payload shapes
├── Data/                     # TelemetryDbContext (EF Core)
├── Migrations/               # EF Core database migrations
├── Models/                   # Domain models (storage-ready)
├── Normalize/                # TelemetryNormalizer — type conversion logic
├── Services/                 # TelemetryProcessor (IHostedService) + SensorService
├── Validation/               # TelemetryValidator — required field checks
├── Program.cs                # DI registration, channel setup, middleware config
├── SensorMonitor.csproj      # .NET 10 project file
└── appsettings.json          # Connection string config
```

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- PostgreSQL (local instance or a hosted service)
- Visual Studio 2022 or VS Code

### Setup

1. Clone the repository:
   ```bash
   git clone https://github.com/thorstore-ong/sensor-monitor-microservice.git
   cd sensor-monitor-microservice
   ```

2. Set your PostgreSQL connection string. In `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=sensor_db;Username=your_user;Password=your_password"
     }
   }
   ```
   Or use [.NET User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) (already configured — `UserSecretsId` is set in the `.csproj`):
   ```bash
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=localhost;..."
   ```

3. Run the project — migrations apply automatically at startup:
   ```bash
   dotnet run
   ```

4. Open the Swagger UI to explore and test endpoints:
   ```
   http://localhost:5201/swagger
   ```

---

## NuGet Packages

| Package | Version | Purpose |
|---|---|---|
| `Microsoft.AspNetCore.OpenApi` | 10.0.1 | OpenAPI support |
| `Microsoft.EntityFrameworkCore` | 10.0.1 | ORM |
| `Microsoft.EntityFrameworkCore.Design` | 10.0.1 | EF Core tooling (migrations) |
| `Npgsql.EntityFrameworkCore.PostgreSQL` | 10.0.0 | PostgreSQL driver for EF Core |
| `Swashbuckle.AspNetCore` | 10.1.0 | Swagger UI |

---

## Context

This project was built to mirror real workflows used by the IoT.nxt data acquisition team. It gave hands-on exposure to:

- How ASP.NET Core Web APIs act as ingestion points for sensor telemetry
- The role of DTOs and domain models in a data transformation pipeline
- How `System.Threading.Channels` enables event-driven, decoupled processing without an external broker
- How physical equipment (generators, meters, rectifiers) produces operational data that managers use to monitor performance
