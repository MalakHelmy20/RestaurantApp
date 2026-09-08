# My Restaurant App
ASP.NET Core backend for a multi-restaurant ordering platform. The API models real restaurant operations: who can do what, when a kitchen is open, and what the database is never allowed to accept.
## Features
- **Layer-Based Architecture** — API, Application, Domain, and Infrastructure stay separated
- **JWT auth** with three roles: `SystemAdmin`, `RestaurantOwner`, `Customer`
- **BCrypt** password hashing
- **Role-aware orders** — customers see their own, owners see their restaurants, admins see everything
- **Order lifecycle** — `confirm` → `preparing` → `onDelivery` → `delivered` / `cancelled`
- **Opening hours** — orders are rejected when the restaurant is closed, including overnight windows (e.g. 22:00–02:00)
- **Soft delete + audit** — `IsDeleted`, `CreatedBy`, `UpdatedBy`, `DeletedAt` on core entities
- **Ratings** — one rating per user per restaurant, values constrained to 1–5
- **Search, filters, pagination** for restaurants and products
- **EF Core** global query filters, unique indexes, check constraints, and `Restrict` delete behavior
- **OpenAPI / Swagger** in Development
- **Centralized exception handling** mapped to HTTP status codes
## Architecture
```
┌─────────────────────────────────────────────┐
│                 API                         │
│   Controllers · JWT · exception pipeline    │
└─────────────────────┬───────────────────────┘
                      │
┌─────────────────────▼───────────────────────┐
│              Application                    │
│   Services · DTOs · mapping · interfaces    │
└─────────────────────┬───────────────────────┘
                      │
        ┌─────────────┴─────────────┐
        ▼                           ▼
┌───────────────┐         ┌───────────────────┐
│    Domain     │         │  Infrastructure   │
│  entities &   │         │  EF Core, SQL     │
│  business     │         │  Server, repos,   │
│  rules        │         │  migrations       │
└───────────────┘         └───────────────────┘
```
Controllers stay thin. Domain rules live in services and entities. Persistence is isolated behind repository interfaces.
| Project | Responsibility |
| --- | --- |
| `MyRestaurantApp.Api` | HTTP, auth, DI, Swagger |
| `MyRestaurantApp.Application` | Use cases, DTOs, mapping extensions |
| `MyRestaurantApp.Domain` | Entities, enums, opening-hours logic |
| `MyRestaurantApp.Infrastructure` | `AppDbContext`, repositories, migrations |
## Domain
```
User 1──* Restaurant (owner)
User 1──* Order (customer)
User 1──* Rating
Restaurant 1──* Product
Restaurant 1──* Order
Restaurant 1──* Rating
Restaurant *──* Category   (RestaurantCategory join)
Product *──* OrderItem *──1 Order
```
### Roles
| Role | Typical access |
| --- | --- |
| `Customer` | Register, place orders, update/cancel while status is `confirm`, rate a restaurant once |
| `RestaurantOwner` | Manage own restaurants, products, and categories; update order status |
| `SystemAdmin` | Full access, including user management and restaurant deletion |
The **first registered user** is promoted to `SystemAdmin`. Later registrations default to `Customer`.
### Order rules
- Only `Customer` can create an order
- Restaurant must be **open now**
- Every line item must belong to that restaurant and be available
- Customers may change items or cancel only while status is `confirm`
- Owners and admins update **status**, not line items
- Customers cannot view another customer's order; owners cannot view another restaurant's orders
### Data integrity
- Soft delete via `IsDeleted` plus EF Core **global query filters**
- Unique filtered index: one active rating per `(UserId, RestaurantId)`
- Check constraint: `RatingValue` between 1 and 5
- Foreign keys use `DeleteBehavior.Restrict` so related rows are not cascade-wiped
- Roles and order status stored as strings in SQL
## Tech stack
- .NET 10 / C# / ASP.NET Core Web API
- Entity Framework Core 10 + SQL Server
- JWT Bearer authentication
- BCrypt.Net-Next
- OpenAPI + Swagger UI
## Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server (local instance with `Trusted_Connection` is the default)
- [EF Core tools](https://learn.microsoft.com/ef/core/cli/dotnet) (`dotnet tool install --global dotnet-ef`) if you need to add migrations
## Getting started
### 1. Clone and restore
```bash
git clone <your-repo-url>
cd restaurant-app
dotnet restore MyRestaurantApp.slnx
```
### 2. Connection string
Default in `MyRestaurantApp.Api/appsettings.json`:
```json
"DefaultConnection": "Server=.;Database=MyRestaurantDb;Trusted_Connection=True;TrustServerCertificate=True;"
```
Change `Server` if you are not using the local default instance.
JWT settings live under the `Jwt` section (`Key`, `Issuer`, `Audience`). Use a long secret in any shared or production environment.
### 3. Apply migrations
```bash
dotnet ef database update --project MyRestaurantApp.Infrastructure --startup-project MyRestaurantApp.Api
```
### 4. Run the API
```bash
dotnet run --project MyRestaurantApp.Api --launch-profile http
```
| | URL |
| --- | --- |
| API | http://localhost:5140 |
| Swagger UI | http://localhost:5140/swagger |
| OpenAPI JSON | http://localhost:5140/openapi/v1.json |
HTTPS profile: `https://localhost:7193` (`--launch-profile https`).
## Quick start with the API
1. `POST /api/User/register` — first account becomes `SystemAdmin`
2. Copy the `token` from the response
3. Send `Authorization: Bearer <token>` on protected routes
4. Create restaurants, categories, and products
5. Register a customer (or have an admin create one) and `POST /api/Order`
### Register
```http
POST /api/User/register
Content-Type: application/json
{
  "firstName": "Ahmad",
  "lastName": "Ali",
  "email": "ahmad@example.com",
  "phone": "0590000000",
  "password": "Secret1!",
  "confirmPassword": "Secret1!",
  "address": "Nablus"
}
```
Password must be at least 6 characters, with one uppercase letter and one special character.
