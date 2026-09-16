# NZ Walks API

A REST API for managing New Zealand walking trails and their regions, built with ASP.NET Core 8 and Entity Framework Core.

> Built by following the "ASP.NET Core Web API" Udemy course. The course provides the project brief and overall direction; this repository is my implementation of it. I'm keeping it public because it demonstrates the patterns I learned — JWT auth, the repository pattern, EF Core migrations, and DTO mapping.

## Features

- **JWT authentication** with ASP.NET Core Identity, including user registration and login
- **Role-based authorization** — `Reader` role for GET endpoints, `Writer` role for create/update/delete
- **Repository pattern** — controllers depend on interfaces (`IRegionRepository`, `IWalkRepository`, `IImageRepository`, `ITokenRepository`), keeping data access out of the controllers
- **DTO layer with AutoMapper** so domain models are never exposed directly over the wire
- **Filtering, sorting, and pagination** on the walks endpoint
- **Image upload** with local file storage and validation
- **Custom model-validation action filter** (`ValidateModelAttribute`) to avoid repeating `ModelState` checks
- **Serilog logging** to both console and daily rolling files
- **Swagger UI** configured with JWT bearer auth, so protected endpoints can be tested from the browser

## Tech stack

- ASP.NET Core 8 (Web API)
- Entity Framework Core 9 + SQL Server
- ASP.NET Core Identity
- AutoMapper
- Serilog
- Swashbuckle / Swagger

## Architecture

The project uses two separate `DbContext`s:

- `NZWalksDbContext` — domain data (regions, walks, difficulties, images)
- `NZWalksAuthDbContext` — Identity data (users, roles)

Request flow: **Controller → Repository (interface) → EF Core → SQL Server**, with AutoMapper converting between DTOs and domain models at the controller boundary.

## Endpoints

### Auth
| Method | Route | Description |
|---|---|---|
| POST | `/api/auth/register` | Register a new user |
| POST | `/api/auth/login` | Log in and receive a JWT |

### Regions
| Method | Route | Role |
|---|---|---|
| GET | `/api/regions` | Reader |
| GET | `/api/regions/{id}` | Reader |
| POST | `/api/regions` | Writer |
| PUT | `/api/regions/{id}` | Writer |
| DELETE | `/api/regions/{id}` | Writer |

### Walks
| Method | Route | Notes |
|---|---|---|
| GET | `/api/walks` | Supports `filterOn`, `filterQuery`, `sortBy`, `isAscending`, `pageNumber`, `pageSize` |
| GET | `/api/walks/{id}` | |
| POST | `/api/walks` | |
| PUT | `/api/walks/{id}` | |
| DELETE | `/api/walks/{id}` | |

### Images
| Method | Route | Description |
|---|---|---|
| POST | `/api/images/upload` | Upload an image file |

## Getting started

### Prerequisites
- .NET 8 SDK
- SQL Server (LocalDB or a full instance)

### Setup

1. Clone the repository and open `NZWalks.sln`.

2. Set your connection strings in `NZWalks.API/appsettings.json`, replacing `YOUR_SERVER` with your SQL Server instance.

3. Set a JWT signing key. It is intentionally left blank in `appsettings.json` — supply it via user-secrets rather than committing it:

   ```bash
   cd NZWalks.API
   dotnet user-secrets init
   dotnet user-secrets set "Jwt:Key" "a-key-at-least-32-characters-long"
   ```

4. Apply the migrations for both contexts:

   ```bash
   dotnet ef database update --context NZWalksDbContext
   dotnet ef database update --context NZWalksAuthDbContext
   ```

5. Run it:

   ```bash
   dotnet run
   ```

   Swagger UI is available at `https://localhost:7203/swagger` in development.

### Using protected endpoints

Register a user, log in to get a token, then click **Authorize** in Swagger and paste the token. Note that roles (`Reader` / `Writer`) must be assigned to the user in the auth database for role-protected endpoints to succeed.
