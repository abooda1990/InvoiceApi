# Invoice Approval API

A REST API for approving supplier invoices inside a company. Employees log in, see the invoices assigned to them, and approve, reject, forward or comment on them. Admins can see everything and delete invoices that were not approved.

It is a rebuild of my bachelor project (an invoice approval app for Microsoft Teams), written from scratch to learn ASP.NET Core properly.

## Tech stack

- **.NET 8 / ASP.NET Core** Web API with controllers
- **Entity Framework Core 8** with SQLite (code-first migrations, seed data)
- **JWT bearer authentication** with role-based authorization
- **ASP.NET Core Identity `PasswordHasher`** for password hashes
- **NUnit** unit tests with hand-written fake repositories
- **Swagger / OpenAPI** with a login button for testing

## Architecture

```
Controller  →  Service  →  Repository  →  AppDbContext (EF Core)  →  SQLite
   HTTP         business      data access
               rules
```

| Layer | Folder | Responsibility |
|---|---|---|
| Controllers | `Controllers/` | HTTP only: routes, status codes, reads the current user from the JWT |
| Services | `Services/` | Business rules: who may approve, which statuses can change, token creation |
| Repositories | `Repositories/` | All EF Core queries (`Include`, filtering). The only layer that touches `AppDbContext` |
| DTOs | `DTOs/` | Request and response shapes, with validation attributes |
| Middleware | `Middleware/` | Turns custom exceptions into status codes (`404`, `403`, `400`, `401`, `500`) |

Every layer depends on interfaces, registered with dependency injection in `Program.cs`. That is what lets the unit tests swap the real repositories for in-memory fakes.

## Business rules

- An invoice starts as **Pending**, and can become **Approved**, **Rejected** or **Forwarded**.
- Only the **responsible user** (or an **Admin**) can update, approve, reject or forward an invoice.
- Only **Pending** and **Forwarded** invoices can be changed.
- A **Viewer** can never be responsible for an invoice.
- Comments need view access to the invoice.
- Only an **Admin** can delete, and **Approved** invoices can't be deleted.

## Getting started

### Requirements

- .NET 8 SDK or newer

### Run it

```powershell
git clone https://github.com/abooda1990/InvoiceApi.git
cd InvoiceApi\src\InvoiceApi

# The JWT signing key is not in the repository. Set your own for local development:
dotnet user-secrets set "Jwt:Key" "<a long random string, at least 32 characters>"

dotnet run
```

Open **http://localhost:5000/swagger**.

On startup the app applies the EF Core migrations and seeds test data, so `invoices.db` is created automatically.

### Test users

| Email | Password | Role |
|---|---|---|
| `karim@example.no` | `Karim123!` | Admin |
| `kari@example.no` | `Kari123!` | Approver |
| `ola@example.no` | `Ola123!` | Approver |
| `ingrid@example.no` | `Ingrid123!` | Viewer |

In Swagger: call `POST /api/auth/login`, copy the `token`, click **Authorize**, and paste it.

## Endpoints

All endpoints except login need a JWT (`Authorization: Bearer <token>`).

| Method | Route | Who | Description |
|---|---|---|---|
| `POST` | `/api/auth/login` | Anyone | Returns a JWT and the user |
| `GET` | `/api/invoices?userId=&status=` | Logged in | List invoices, with optional filters |
| `GET` | `/api/invoices/{id}` | Logged in | One invoice |
| `GET` | `/api/invoices/{id}/comments` | Logged in | Comments on an invoice |
| `POST` | `/api/invoices` | Logged in | Create an invoice (starts as Pending) |
| `PUT` | `/api/invoices/{id}` | Responsible user / Admin | Update details |
| `POST` | `/api/invoices/{id}/approve` | Approver, Admin | Approve |
| `POST` | `/api/invoices/{id}/reject` | Approver, Admin | Reject |
| `POST` | `/api/invoices/{id}/forward` | Responsible user / Admin | Forward to another user |
| `POST` | `/api/invoices/{id}/comments` | Users with view access | Add a comment |
| `DELETE` | `/api/invoices/{id}` | Admin | Delete (not if Approved) |
| `GET` | `/api/users` | Logged in | List users |
| `GET` | `/api/users/{id}` | Logged in | One user |

### Error responses

| Status | When |
|---|---|
| `400` | Validation failed, or a business rule was broken (for example approving an already approved invoice) |
| `401` | Missing, invalid or expired token, or wrong email or password |
| `403` | Logged in, but not allowed (wrong role, or not responsible for the invoice) |
| `404` | Invoice or user not found |

## Tests

```powershell
dotnet test
```

The tests cover the rules in `InvoiceService`: approving, rejecting by the wrong user, Admin overrides, not-found cases, and deleting. They run against fake repositories in memory, so no database is needed.

## Project structure

```
InvoiceApi/
├── src/InvoiceApi/
│   ├── Controllers/      AuthController, InvoicesController, UsersController
│   ├── Services/         InvoiceService, AuthService, TokenService, UserService
│   ├── Repositories/     InvoiceRepository, UserRepository
│   ├── Data/             AppDbContext, DbSeeder
│   ├── Models/           User, UserInfo, Invoice, InvoiceComment, InvoiceViewAccess
│   ├── DTOs/             Request and response records
│   ├── Middleware/       ErrorHandlingMiddleware
│   ├── Exceptions/       NotFound, Forbidden, BusinessRule, Unauthorized
│   ├── Mappings/         Model → DTO extension methods
│   └── Migrations/       EF Core migrations
└── tests/InvoiceApi.Tests/
    ├── Fakes/            In-memory repositories
    └── InvoiceService.Tests.cs
```
