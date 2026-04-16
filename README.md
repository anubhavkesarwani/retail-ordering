<div align="center">

# 🍕 Retail Ordering System

**A full-stack retail ordering platform for pizza, cold drinks, and artisan breads**

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?style=for-the-badge&logo=dotnet)
![Angular](https://img.shields.io/badge/Angular-21-DD0031?style=for-the-badge&logo=angular)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-17-4169E1?style=for-the-badge&logo=postgresql)
![TypeScript](https://img.shields.io/badge/TypeScript-5.9-3178C6?style=for-the-badge&logo=typescript)

</div>

---

## ✨ Features

- 🔐 **JWT Authentication** — Register, login, and protected routes
- 🛍️ **Product Catalogue** — Browse with filter by Brand, Category, and Packaging
- 🛒 **Shopping Cart** — Add, update, remove items; real-time cart count in navbar
- 📦 **Order Management** — Place orders, track status, cancel pending orders
- 🎁 **Packaging Types** — Box, Bottle, Bag, Wrap, Tin — filterable on the menu
- 👤 **User Profile** — View account details and order history
- 🔒 **Role-Based Access** — Admin and Customer roles

---

## 🏗️ Architecture

Clean Architecture with four layers:

```
RetailOrdering.Core            ← Entities, Interfaces, Enums (no dependencies)
RetailOrdering.Application     ← DTOs, Services, Business Logic
RetailOrdering.Infrastructure  ← EF Core, Repositories, Migrations
RetailOrdering.Api             ← ASP.NET Controllers, Middleware, DI wiring
retail-ordering-frontend/      ← Angular 21 SPA (Standalone Components, Zoneless)
```

---

## 🛠️ Tech Stack

### Backend
| Technology | Version | Purpose |
|-----------|---------|---------|
| ASP.NET Core | .NET 10 | REST API, JWT auth, middleware |
| Entity Framework Core | 10.x | ORM, code-first migrations |
| PostgreSQL | 17 | Primary database |
| FluentValidation | — | Request validation |

### Frontend
| Technology | Version | Purpose |
|-----------|---------|---------|
| Angular | 21 | SPA framework (standalone, zoneless) |
| TypeScript | 5.9 | Type-safe component logic |
| RxJS | 7.8 | Reactive HTTP and state streams |
| Angular Router | — | Lazy-loaded feature routes |

---

## 📂 Project Structure

```
hackathon/
├── RetailOrdering.Api/
│   ├── Controllers/          # Auth, Products, Cart, Orders
│   ├── Middleware/           # Exception handling
│   ├── Properties/           # Launch settings
│   ├── appsettings.json      # DB connection & JWT config
│   └── Program.cs            # DI, middleware pipeline
│
├── RetailOrdering.Application/
│   ├── DTOs/                 # Request/response models
│   └── Services/             # AuthService, ProductService, CartService, OrderService
│
├── RetailOrdering.Core/
│   ├── Entities/             # User, Product, Brand, Category, Packaging, Order, ...
│   ├── Interfaces/           # Repository contracts
│   └── Enums/                # OrderStatus, UserRole
│
├── RetailOrdering.Infrastructure/
│   ├── Data/                 # ApplicationDbContext, EF config
│   ├── Migrations/           # EF Core migration history
│   └── Repositories/        # Concrete repository implementations
│
└── retail-ordering-frontend/
    └── src/app/
        ├── core/             # Services, models, interceptors
        ├── features/         # Home, Products, Cart, Checkout, Orders, Auth, Profile
        └── shared/           # Navbar, Footer components
```

---

## 🗄️ Database Schema

```
Users ──────────────────────────────────────────────────────┐
  Id, Email, PasswordHash, FirstName, LastName, Role        │
                                                            │
Brands                                                      │
  Id, Name, Description, ImageUrl, IsActive                 │
    │                                                        │
    └── Categories                                          │
          Id, Name, BrandId                                 │
              │                                             │
              └── Products ──────── Packagings              │
                    Id, Name, Price  Id, Name               │
                    CategoryId, PackagingId                 │
                    StockQuantity, ImageUrl                  │
                       │                                     │
                       ├── CartItems (UserId FK) ───────────┤
                       │     Id, UserId, ProductId, Qty     │
                       │                                     │
                       └── OrderItems                        │
                             Id, OrderId, ProductId         │
                             Quantity, UnitPrice             │
                                │                            │
                             Orders (UserId FK) ────────────┘
                               Id, UserId, Status
                               TotalAmount, DeliveryAddress
```

### Tables & Current Seed Data
| Table | Rows | Description |
|-------|------|-------------|
| `Users` | 4 | Registered accounts |
| `Brands` | 3 | Pizza Palace, CoolSip Co., BreadCraft |
| `Categories` | 6 | Classic Pizzas, Specialty Pizzas, Cold Drinks, Energy Drinks, White Breads, Whole Grain |
| `Packagings` | 5 | Box, Bottle, Bag, Wrap, Tin |
| `Products` | 10 | Margherita, Pepperoni, BBQ Chicken, Truffle Mushroom, Cola, Lemon Water, Mango Blast, Sourdough, Ciabatta, Multigrain |
| `Orders` | 10 | Sample orders across users |
| `CartItems` | varies | Active cart sessions |

---

## 🚀 Getting Started

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js 22+](https://nodejs.org/) & npm 11+
- [PostgreSQL 17](https://www.postgresql.org/)
- [Angular CLI 21](https://angular.dev/tools/cli) — `npm install -g @angular/cli`

### 1. Clone & Setup

```bash
git clone <repository-url>
cd hackathon
```

### 2. Configure Database

```bash
# Start PostgreSQL
sudo systemctl start postgresql

# Create the database
psql -U postgres -c 'CREATE DATABASE "RetailOrdering";'

# Apply EF Core migrations (creates all tables)
dotnet ef database update \
  --project RetailOrdering.Infrastructure \
  --startup-project RetailOrdering.Api
```

### 3. Configure App Settings

Edit `RetailOrdering.Api/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=RetailOrdering;Username=postgres;Password=yourpassword"
  },
  "Jwt": {
    "Key": "your-secret-key-min-32-chars",
    "Issuer": "RetailOrdering",
    "Audience": "RetailOrderingUsers",
    "ExpiryMinutes": "60"
  }
}
```

### 4. Start the API

```bash
cd /path/to/hackathon
dotnet run --project RetailOrdering.Api --urls "http://localhost:5124"
```

API will be live at **http://localhost:5124**
Swagger docs at **http://localhost:5124/swagger**

### 5. Start the Frontend

```bash
cd retail-ordering-frontend
npm install
npm start
```

Frontend will be live at **http://localhost:4200**

---

## 🔌 API Endpoints

### Authentication
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| `POST` | `/api/auth/register` | ❌ | Register new user |
| `POST` | `/api/auth/login` | ❌ | Login, returns JWT |

### Products
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| `GET` | `/api/products` | ❌ | All products |
| `GET` | `/api/products/{id}` | ❌ | Product by ID |
| `GET` | `/api/products/brands` | ❌ | All brands |
| `GET` | `/api/products/categories` | ❌ | All categories |
| `GET` | `/api/products/packagings` | ❌ | All packaging types |
| `GET` | `/api/products/brand/{id}` | ❌ | Products by brand |
| `GET` | `/api/products/category/{id}` | ❌ | Products by category |
| `GET` | `/api/products/packaging/{id}` | ❌ | Products by packaging |

### Cart
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| `GET` | `/api/cart` | ✅ | Get user's cart |
| `POST` | `/api/cart/items` | ✅ | Add item to cart |
| `PUT` | `/api/cart/items/{id}` | ✅ | Update item quantity |
| `DELETE` | `/api/cart/items/{id}` | ✅ | Remove item |
| `DELETE` | `/api/cart` | ✅ | Clear entire cart |

### Orders
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| `POST` | `/api/orders` | ✅ | Place order from cart |
| `GET` | `/api/orders` | ✅ | List user's orders |
| `GET` | `/api/orders/{id}` | ✅ | Order detail |
| `PUT` | `/api/orders/{id}/cancel` | ✅ | Cancel pending order |

---

## 🧩 EF Core Migrations

```bash
# Add a new migration after model changes
dotnet ef migrations add <MigrationName> \
  --project RetailOrdering.Infrastructure \
  --startup-project RetailOrdering.Api

# Apply to database
dotnet ef database update \
  --project RetailOrdering.Infrastructure \
  --startup-project RetailOrdering.Api

# Applied migrations
dotnet ef migrations list \
  --project RetailOrdering.Infrastructure \
  --startup-project RetailOrdering.Api
```

### Migration History
| Migration | Description |
|-----------|-------------|
| `20260416065936_InitialCreate` | Full initial schema |
| `20260416090747_AddPackaging` | Packagings table + ProductId FK |

---

## 🔑 Environment Files

### `retail-ordering-frontend/src/environments/environment.ts`
```typescript
export const environment = {
  production: false,
  apiUrl: 'http://localhost:5124/api'
};
```

### `retail-ordering-frontend/src/environments/environment.prod.ts`
```typescript
export const environment = {
  production: true,
  apiUrl: '/api'
};
```

---

## 📝 Notes

- **Zoneless Angular**: This app uses Angular 21's zoneless change detection (`provideZonelessChangeDetection()`). All async subscriber callbacks call `this.cdr.markForCheck()` to trigger re-renders.
- **HTTPS**: `UseHttpsRedirection()` is disabled in development. Re-enable it for production deployments.
- **JWT**: Tokens expire after 60 minutes. The JWT interceptor auto-redirects to `/login` on 401 responses.
- **CORS**: Configured to allow `http://localhost:4200` and `http://localhost:4300`.

---

<div align="center">

Built with ❤️ as a hackathon project

</div>
