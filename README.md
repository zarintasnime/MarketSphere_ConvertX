# MarketSphere ConvertX

> A full-stack FMCG sales and distribution management platform built with ASP.NET Core, Angular, React, EF Core, and SQL Server.

MarketSphere ConvertX is a portfolio-focused business operations platform designed around realistic FMCG sales, inventory, procurement, fulfilment, CRM, payment, approval, and management workflows.

The project uses a **Clean Architecture-inspired ASP.NET Core backend**, an **Angular operations portal** for transactional workflows, and a separate **React management dashboard** for analytics and executive visibility.

Demo and seed data are localized for **Bangladesh** to make the application more realistic for portfolio demonstrations, technical interviews, and GitHub reviews.

---

## Tech Stack

### Backend

* ASP.NET Core 9
* C#
* Entity Framework Core 9
* SQL Server
* JWT authentication
* Role and permission-based authorization
* Clean Architecture-inspired structure
* Audit logging
* Soft delete
* Approval workflows
* Idempotent business operations
* EF Core migrations
* Development seeders

### Operations Frontend

* Angular 21
* TypeScript
* Responsive business UI
* Transactional workflow screens
* Authentication and route protection
* Shared API integration

### Management Frontend

* React 19
* TypeScript
* Vite
* Executive dashboards
* KPI and business analytics
* Management reporting
* Approval and monitoring views

---

## High-Level Architecture

```text
                    ┌─────────────────────────┐
                    │   Angular Operations    │
                    │        Portal           │
                    │   localhost:4200        │
                    └────────────┬────────────┘
                                 │
                                 │ REST API
                                 │
┌─────────────────────────┐      ▼
│   React Management      │ ┌─────────────────────────┐
│       Dashboard         ├▶│    ASP.NET Core API     │
│   localhost:5173        │ │    localhost:5080       │
└─────────────────────────┘ └────────────┬────────────┘
                                        │
                                        │ EF Core
                                        ▼
                              ┌─────────────────────────┐
                              │       SQL Server        │
                              │ MarketSphereConvertXDb  │
                              └─────────────────────────┘
```

---

## Core Business Areas

MarketSphere ConvertX goes beyond basic CRUD operations and models interconnected business workflows.

### CRM & Organization

* Clients and customers
* Suppliers
* Organizational structure
* Territories and regions
* User management
* Roles and permissions

### Product & Pricing

* Product catalog
* SKU management
* Price lists
* Business pricing workflows
* Product availability

### Procurement

* Supplier management
* Procurement workflows
* Purchase-related operations
* Warehouse receiving flows

### Inventory

* Warehouses
* Stock balances
* Stock movements
* Stock reservations
* Stock transfers
* Batch tracking
* FEFO-oriented inventory handling
* Inventory health monitoring

### Sales & Orders

* Customer orders
* Order lifecycle management
* Fulfilment workflows
* Delivery-related operations
* Returns
* Payment tracking

### Approvals & Administration

* Approval workflows
* Permission-based actions
* Audit/status history
* Administration screens
* Notifications

### Management Analytics

* Executive KPIs
* Sales performance
* Inventory health
* Delivery and return reporting
* Funnel analysis
* ROI-related reporting
* Approval queues
* Management dashboards

---

## Bangladesh Demo Data

Development seeders contain fictional Bangladesh-focused demo data so the system feels realistic immediately after setup.

Examples include:

* Dhaka
* Chattogram
* Mirpur
* Agrabad
* Tejgaon
* Jatrabari
* Bangladesh-style suppliers
* Local customer/outlet records
* Warehouse records
* BDT-based pricing
* FMCG-oriented products and SKUs

> All seeded business names and records are intended for development/demo purposes.

---

## Repository Structure

```text
MarketSphere_ConvertX/
│
├── .github/
│   └── workflows/
│
├── backend/
│   ├── MarketSphere.Api/
│   ├── MarketSphere.Application/
│   ├── MarketSphere.Domain/
│   ├── MarketSphere.Infrastructure/
│   └── MarketSphereConvertX.sln
│
├── frontend/
│   ├── angular-operations/
│   └── react-management/
│
├── docs/
│   ├── ARCHITECT_REVIEW.md
│   ├── DESIGN_REVIEW.md
│   ├── IMPROVEMENT_PLAN.md
│   ├── CV_PROJECT_ENTRY.md
│   └── REVIEW_SUMMARY.md
│
├── .gitignore
├── README.md
├── RUN_LOCAL.md
├── START_HERE.txt
├── setup-local.cmd
├── start-local.cmd
└── start-local.sh
```

---

# Getting Started

## Prerequisites

Install the following before running the project locally:

* **.NET SDK 9**
* **SQL Server**
* **Node.js 22+**
* **npm**
* Visual Studio 2022+ or VS Code
* SQL Server Management Studio (optional)

Docker is **not required** for this project.

---

## 1. Clone the Repository

```bash
git clone https://github.com/YOUR_USERNAME/MarketSphere-ConvertX.git
cd MarketSphere-ConvertX
```

If you downloaded the project as a ZIP, extract it and open the project root folder.

---

## 2. Configure Local Development Secrets

On Windows, run:

```bat
setup-local.cmd
```

The script asks you to choose a local bootstrap administrator password.

The administrator email used by the development seed is:

```text
admin@marketsphere.local
```

The password is **not committed to GitHub**.

`setup-local.cmd` stores sensitive development configuration using **.NET User Secrets** instead of placing secrets directly inside `appsettings.json`.

Typical development secrets include:

```text
Jwt:SigningKey
BootstrapAdmin:Password
```

You can verify configured user secrets with:

```bash
dotnet user-secrets list --project backend/MarketSphere.Api/MarketSphere.Api.csproj
```

---

## 3. Configure SQL Server

The default local connection string is:

```text
Server=.;
Database=MarketSphereConvertXDb;
Trusted_Connection=True;
TrustServerCertificate=True;
MultipleActiveResultSets=True;
```

This works when your local SQL Server instance can be accessed using:

```text
.
```

or:

```text
localhost
```

If you use SQL Server Express, you may need:

```text
.\SQLEXPRESS
```

Update the development connection string accordingly.

---

## 4. Run the Backend

From the repository root:

```bash
dotnet restore backend/MarketSphereConvertX.sln
```

Then run:

```bash
dotnet run --project backend/MarketSphere.Api/MarketSphere.Api.csproj --launch-profile http
```

The API should start at:

```text
http://localhost:5080
```

Swagger:

```text
http://localhost:5080/swagger
```

---

## Database Migration & Seed Data

During local Development startup, the backend applies pending EF Core migrations and executes development seeders.

The expected development startup flow is:

```text
Application starts
        ↓
SQL Server connection
        ↓
EF Core migrations
        ↓
Development seed data
        ↓
API becomes available
```

In the normal local workflow, manually running `Update-Database` is not required.

If migration troubleshooting is required, EF Core commands can still be executed manually.

---

## 5. Run the Angular Operations Portal

Open a new terminal:

```bash
cd frontend/angular-operations
npm install
npm start
```

Open:

```text
http://localhost:4200
```

The Angular application is intended primarily for operational and transactional workflows.

---

## 6. Run the React Management Dashboard

Open another terminal:

```bash
cd frontend/react-management
npm install
npm run dev
```

Open:

```text
http://localhost:5173
```

The React application focuses on management dashboards, KPI monitoring, reporting, and executive-level visibility.

---

# Application URLs

| Application                | Local URL                       |
| -------------------------- | ------------------------------- |
| ASP.NET Core API           | `http://localhost:5080`         |
| Swagger UI                 | `http://localhost:5080/swagger` |
| Angular Operations Portal  | `http://localhost:4200`         |
| React Management Dashboard | `http://localhost:5173`         |

---

## Quick Windows Start

After configuring the project once with:

```bat
setup-local.cmd
```

you can use:

```bat
start-local.cmd
```

to start the local development applications.

For manual commands and troubleshooting, see:

```text
RUN_LOCAL.md
```

---

# Authentication

The platform uses JWT-based authentication.

A simplified authentication flow is:

```text
User Login
    ↓
Credential Validation
    ↓
JWT Access Token
    ↓
Authenticated API Request
    ↓
Role / Permission Validation
    ↓
Authorized Business Operation
```

The local development administrator email is:

```text
admin@marketsphere.local
```

No default administrator password is committed to this repository.

The password is configured locally using:

```text
setup-local.cmd
```

The application may require the bootstrap administrator to change the initial password before accessing protected business features.

---

# Security Design

Security-related functionality includes:

* JWT authentication
* Role-based authorization
* Permission-based authorization
* Password hashing
* Refresh-token handling
* Account lockout support
* Audit information
* Protected business operations
* Controlled CORS configuration
* Development secrets outside source-controlled configuration

Production secrets are intentionally not stored in `appsettings.json`.

Example placeholder:

```json
{
  "Jwt": {
    "SigningKey": "SET_IN_USER_SECRETS"
  },
  "BootstrapAdmin": {
    "Password": ""
  }
}
```

Never commit real:

* Passwords
* JWT signing keys
* API secrets
* Production connection strings
* `.env` files containing credentials

---

## Production Security Considerations

Before a production deployment:

* Use a dedicated secret manager
* Rotate signing keys
* Enforce HTTPS
* Tighten CORS origins
* Use production-grade SQL credentials
* Configure structured centralized logging
* Add monitoring and alerting
* Review token lifetime policies
* Consider HttpOnly/Secure cookies or a BFF design for browser refresh-token handling
* Add rate limiting where appropriate
* Review account lockout policies

---

# Engineering Highlights

The project demonstrates implementation of interconnected application logic rather than isolated CRUD screens.

Examples include:

* Permission-based authorization
* JWT authentication
* Audit and status history
* Approval workflows
* Idempotent mutations
* Inventory reservations
* Stock transfers
* Warehouse inventory
* Batch-oriented stock
* FEFO-oriented fulfilment
* Order lifecycle management
* Customer and supplier payment workflows
* Returns management
* Campaign attribution
* KPI projections
* Executive analytics
* Separate operational and management frontends

---

# Backend Architecture

The backend is divided into four primary projects.

## MarketSphere.Domain

Contains core domain concepts such as:

* Entities
* Domain rules
* Constants
* Domain exceptions
* Shared domain abstractions

The Domain project does not depend on infrastructure concerns.

---

## MarketSphere.Application

Contains:

* Application services
* Business use cases
* DTOs
* Interfaces
* Authorization-oriented logic
* Application-level validation
* Module-specific workflows

---

## MarketSphere.Infrastructure

Contains implementation details such as:

* EF Core
* SQL Server persistence
* Database configuration
* Migrations
* Seeders
* Infrastructure services
* Repository/persistence concerns

---

## MarketSphere.Api

Acts as the application entry point and contains:

* Controllers
* Authentication configuration
* Authorization configuration
* Middleware
* Dependency injection
* Swagger
* API contracts
* Application startup

---

# Frontend Architecture

## Angular Operations Portal

Designed for employees responsible for daily operational workflows.

Typical responsibilities include:

* CRM
* Products
* Pricing
* Procurement
* Inventory
* Orders
* Fulfilment
* Returns
* Payments
* Administration
* Notifications
* Field operations

---

## React Management Dashboard

Designed primarily for management and reporting scenarios.

Typical responsibilities include:

* Executive dashboard
* KPI analysis
* Business performance
* Sales analysis
* Inventory health
* Delivery performance
* Return analysis
* Approval monitoring
* Funnel reporting
* ROI-oriented analytics

---

# UI/UX Direction

The interface is designed around business software principles:

* Clear visual hierarchy
* Readable data tables
* Consistent spacing
* Shared design tokens
* Responsive layouts
* Explicit action hierarchy
* Human-readable business labels
* Reduced dependence on raw database IDs
* Accessible form states
* Consistent operational workflow patterns

Inventory-related screens use human-readable information such as:

```text
Warehouse Name
SKU Code
SKU Name
Batch Number
```

instead of displaying only internal identifiers.

---

# Screenshots

Recommended screenshots for the GitHub repository:

1. Login screen
2. Angular Operations Dashboard
3. React Executive Dashboard
4. Inventory Management
5. Order Fulfilment
6. CRM / Client Management
7. Approval Queue
8. Analytics / KPI View

A suggested repository structure is:

```text
docs/
└── screenshots/
    ├── login.png
    ├── operations-dashboard.png
    ├── management-dashboard.png
    ├── inventory.png
    ├── order-fulfilment.png
    └── approvals.png
```

Then screenshots can be embedded in this README.

<!--
Example:

![Operations Dashboard](docs/screenshots/operations-dashboard.png)

![Management Dashboard](docs/screenshots/management-dashboard.png)
-->

---

# Architecture & Design Reviews

Additional engineering documentation is available under `docs/`.

### Architecture Review

```text
docs/ARCHITECT_REVIEW.md
```

Covers:

* Business logic
* Architecture
* Security
* Code consistency
* Scalability
* Maintainability
* Technical debt

### UI/UX Review

```text
docs/DESIGN_REVIEW.md
```

Covers:

* Table consistency
* Visual hierarchy
* Design token usage
* Responsive behavior
* Operational UX
* Management UI consistency

### Improvement Roadmap

```text
docs/IMPROVEMENT_PLAN.md
```

Contains staged recommendations for improving the application further.

### CV Project Entry

```text
docs/CV_PROJECT_ENTRY.md
```

Contains portfolio and CV-oriented project wording.

### Review Summary

```text
docs/REVIEW_SUMMARY.md
```

Summarizes applied changes and remaining priorities.

---

# Current Quality Status

The repository includes a GitHub Actions workflow intended to validate project builds.

Key areas for continued engineering improvement include:

* Unit testing
* Integration testing
* Authentication tests
* Authorization tests
* Inventory invariant tests
* Order state-transition tests
* Payment workflow tests
* Approval workflow tests
* Concurrency protection
* Expanded validation
* Production observability

---

# Recommended Testing Strategy

A mature test suite should include:

### Unit Tests

* Authentication rules
* Permission checks
* Order state transitions
* Inventory calculations
* Stock reservations
* Payment rules
* Approval rules

### Integration Tests

* API authentication
* Protected endpoints
* Database persistence
* Order workflows
* Inventory transfers
* User authorization boundaries

### Frontend Tests

* Authentication flow
* Route guards
* Form validation
* Table filtering
* Critical workflows

---

# GitHub Contribution Workflow

A conventional development workflow can be used:

```bash
git checkout -b feature/your-feature
```

Make changes, then:

```bash
git add .
git commit -m "feat: add your feature"
```

Push:

```bash
git push origin feature/your-feature
```

Then open a Pull Request.

Suggested commit prefixes:

```text
feat:      new functionality
fix:       bug fix
refactor:  internal code improvement
docs:      documentation
style:     formatting/UI-only change
test:      tests
chore:     maintenance
```

---

# Git Safety

Before committing:

```bash
git status
```

Review staged files:

```bash
git diff --cached
```

Sensitive or generated files should never be committed.

Examples:

```text
.vs/
bin/
obj/
node_modules/
dist/
.env
.env.local
*.user
```

User secrets are stored outside the repository by the .NET User Secrets mechanism.

---

# Known Improvement Areas

Possible future upgrades include:

* Comprehensive automated tests
* Optimistic concurrency controls
* Improved domain state machines
* Expanded domain-event usage
* Better observability
* Centralized structured logging
* Distributed cache support
* Background job processing
* Notification queues
* More granular API documentation
* End-to-end tests
* Deployment pipelines
* Containerization only if deployment requirements later justify it

Docker is intentionally **not required by the current local development workflow**.

---

# Portfolio Value

MarketSphere ConvertX demonstrates experience across:

* Backend API development
* Relational database modelling
* Business rule implementation
* Authentication and authorization
* Enterprise-style application structure
* Angular development
* React development
* TypeScript
* SQL Server
* EF Core
* Inventory systems
* Order management
* Workflow design
* Management analytics
* UI/UX consistency
* Git/GitHub project organization

---

# Suggested GitHub Topics

```text
aspnet-core
dotnet
csharp
angular
react
typescript
sql-server
entity-framework-core
clean-architecture
jwt-authentication
erp
inventory-management
order-management
sales-management
fmcg
full-stack
```

---

# Repository Description

A suitable GitHub repository description is:

> Full-stack FMCG sales & distribution platform built with ASP.NET Core, Angular, React, EF Core and SQL Server.

---

# Disclaimer

This project is primarily intended for:

* Portfolio demonstration
* Software engineering practice
* Architecture exploration
* Technical interviews
* Full-stack development learning

The included business records and seed data are fictional development/demo data.

Additional security, testing, infrastructure, monitoring, backup, disaster recovery, and operational controls would be required before using the platform in a real production business environment.

---

## Author

Developed as a full-stack software engineering portfolio project.

---

## License

Add an appropriate license before distributing or reusing the project publicly.

For an open-source portfolio repository, the MIT License may be suitable depending on your intended usage.
