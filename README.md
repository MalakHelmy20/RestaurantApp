# My Restaurant App

ASP.NET Core backend for a multi-restaurant ordering platform.

The API models real restaurant operations, including authentication and authorization, restaurant opening hours, order workflows, product availability, ratings, and database-level data integrity.

## Features

* **Layer-Based Architecture** — API, Application, Domain, and Infrastructure layers with clear separation of responsibilities
* **JWT Authentication** with three roles: `SystemAdmin`, `RestaurantOwner`, and `Customer`
* **Role-Based Authorization** to control access to restaurants, orders, products, and users
* **BCrypt Password Hashing**
* **Role-Aware Orders** — customers can access their own orders, owners can access orders from their restaurants, and admins have full access
* **Order Lifecycle** — `Confirm` → `Preparing` → `OnDelivery` → `Delivered` / `Cancelled`
* **Opening Hours Validation** — orders are rejected when a restaurant is closed, including overnight working hours such as `22:00–02:00`
* **Order Validation** — every order item must belong to the selected restaurant and be available
* **Soft Delete & Auditing** — `IsDeleted`, `CreatedBy`, `UpdatedBy`, and `DeletedAt` on core entities
* **Restaurant Ratings** — one active rating per user per restaurant, with values restricted from 1 to 5
* **Search, Filtering & Pagination** for restaurants and products
* **EF Core Data Integrity** — global query filters, unique filtered indexes, check constraints, and restricted delete behavior
* **OpenAPI / Swagger** for API documentation
* **Centralized Exception Handling** with appropriate HTTP status codes

## Architecture

The application follows a layered architecture:

```text
┌─────────────────────────────────────────────┐
│                    API                      │
│        Controllers · Auth · DI · Swagger    │
└──────────────────────┬──────────────────────┘
                       │
┌──────────────────────▼──────────────────────┐
│                Application                  │
│       Services · DTOs · Mapping · Use Cases │
└──────────────────────┬──────────────────────┘
                       │
             ┌─────────┴─────────┐
             ▼                   ▼
┌───────────────────┐   ┌────────────────────┐
│      Domain       │   │   Infrastructure   │
│ Entities · Enums  │   │ EF Core · SQL      │
│ Business Rules    │   │ Repositories ·     │
│                   │   │ Migrations          │
└───────────────────┘   └────────────────────┘
```

### Project Responsibilities

| Project                          | Responsibility                                                         |
| -------------------------------- | ---------------------------------------------------------------------- |
| `MyRestaurantApp.Api`            | HTTP endpoints, authentication, dependency injection, Swagger          |
| `MyRestaurantApp.Application`    | Use cases, services, DTOs, mapping extensions, interfaces              |
| `MyRestaurantApp.Domain`         | Entities, enums, and domain-related business rules                     |
| `MyRestaurantApp.Infrastructure` | `AppDbContext`, repositories, SQL Server configuration, and migrations |

Controllers are kept thin, while application services handle use cases and business operations. Persistence concerns are isolated inside the Infrastructure layer.

## Domain

The main relationships between entities are:

```text
User 1 ── * Restaurant      (owner)
User 1 ── * Order           (customer)
User 1 ── * Rating

Restaurant 1 ── * Product
Restaurant 1 ── * Order
Restaurant 1 ── * Rating

Restaurant * ── * Category  (RestaurantCategory)

Product * ── * OrderItem * ── 1 Order
```

### Roles

| Role              | Typical Access                                                               |
| ----------------- | ---------------------------------------------------------------------------- |
| `Customer`        | Register, place orders, update/cancel confirmed orders, and rate restaurants |
| `RestaurantOwner` | Manage owned restaurants, products, categories, and update order status      |
| `SystemAdmin`     | Full system access, including user management and restaurant deletion        |

> **Registration rule:** The first registered user is automatically promoted to `SystemAdmin`. All subsequent registrations default to `Customer`.

### Order Rules

The order system enforces several business rules:

* Only `Customer` users can create orders.
* The restaurant must be open when the order is placed.
* Every order item must belong to the selected restaurant.
* Every ordered product must be available.
* Customers can modify items or cancel an order only while its status is `Confirm`.
* Restaurant owners and admins can update order status but cannot modify order line items.
* Customers cannot view another customer's orders.
* Restaurant owners cannot view orders belonging to other restaurants.

### Data Integrity

Important business rules are also enforced at the database level:

* Soft deletion using `IsDeleted` with EF Core global query filters.
* A unique filtered index ensures only one active rating exists for each `(UserId, RestaurantId)` pair.
* A check constraint ensures `RatingValue` is between `1` and `5`.
* Foreign keys use `DeleteBehavior.Restrict` to prevent unintended cascading deletes.
* Roles and order statuses are stored as strings in SQL Server.

## Tech Stack

* .NET 10
* C#
* ASP.NET Core Web API
* Entity Framework Core 10
* SQL Server
* JWT Bearer Authentication
* BCrypt.Net-Next
* OpenAPI
* Swagger UI

## Prerequisites

Before running the project, make sure you have:

* [.NET 10 SDK](https://dotnet.microsoft.com/download)
* SQL Server
* EF Core CLI tools, if you need to manage migrations:

```bash
dotnet tool install --global dotnet-ef
```

## Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/MalakHelmy20/RestaurantApp.git
cd RestaurantApp
```

Restore the project dependencies:

```bash
dotnet restore MyRestaurantApp.slnx
```

### 2. Configure the Database

The default connection string is located in:

```text
MyRestaurantApp.Api/appsettings.json
```

Example:

```json
"DefaultConnection": "Server=.;Database=MyRestaurantDb;Trusted_Connection=True;TrustServerCertificate=True;"
```

Update the `Server` value if your SQL Server instance uses a different configuration.

JWT settings are stored under the `Jwt` section:

```json
"Jwt": {
  "Key": "...",
  "Issuer": "...",
  "Audience": "..."
}
```

For shared or production environments, use a strong secret and avoid committing sensitive credentials to source control.

### 3. Apply Database Migrations

Run:

```bash
dotnet ef database update \
  --project MyRestaurantApp.Infrastructure \
  --startup-project MyRestaurantApp.Api
```

This creates and updates the database using the existing EF Core migrations.

### 4. Run the API

Run the HTTP profile:

```bash
dotnet run --project MyRestaurantApp.Api --launch-profile http
```

The default endpoints are:

| Resource     | URL                                     |
| ------------ | --------------------------------------- |
| API          | `http://localhost:5140`                 |
| Swagger UI   | `http://localhost:5140/swagger`         |
| OpenAPI JSON | `http://localhost:5140/openapi/v1.json` |
| HTTPS        | `https://localhost:7193`                |

To use the HTTPS profile:

```bash
dotnet run --project MyRestaurantApp.Api --launch-profile https
```

## Quick Start

Once the API is running:

1. Register the first user using `POST /api/User/register`.
2. The first registered user is automatically assigned the `SystemAdmin` role.
3. Copy the JWT `token` returned by the registration/login response.
4. Add the token to protected requests:

```text
Authorization: Bearer <token>
```

5. Create restaurants, categories, and products.
6. Register a customer or create one through the admin account.
7. Use `POST /api/Order` to place an order.

## Example: Register a User

### Request

```http
POST /api/User/register
Content-Type: application/json
```

```json
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

### Password Requirements

Passwords must contain:

* At least 6 characters
* At least one uppercase letter
* At least one special character

## API Documentation

When running the application in the Development environment, Swagger UI is available at:

```text
http://localhost:5140/swagger
```

Swagger provides an interactive interface for exploring and testing the available API endpoints.

## Project Highlights

This project focuses on implementing real backend business rules rather than exposing simple CRUD operations.

Some examples include:

* Authorization based on the relationship between users and resources.
* Restaurant availability based on opening hours, including overnight schedules.
* Order validation before persistence.
* Database constraints that protect important business rules.
* Soft deletion to preserve existing data.
* Centralized exception handling for consistent API responses.
* Separation between HTTP concerns, application logic, domain models, and persistence.

## Repository

GitHub: https://github.com/MalakHelmy20/RestaurantApp
