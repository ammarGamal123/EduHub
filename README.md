# EduHub — School Management API

A **Clean Architecture** (Onion) ASP.NET Core Web API for managing school entities — students, departments, subjects, and instructors. Built with **.NET 8**, **CQRS/MediatR**, **Entity Framework Core**, **FluentValidation**, and **AutoMapper**.

---

## Table of Contents

- [Tech Stack](#tech-stack)
- [Solution Architecture](#solution-architecture)
- [Project Structure](#project-structure)
- [Layer Responsibilities](#layer-responsibilities)
- [CQRS Request Flow](#cqrs-request-flow)
- [Database](#database)
- [Localization](#localization)
- [Getting Started](#getting-started)
- [API Endpoints](#api-endpoints)
- [Key Design Decisions & Quirks](#key-design-decisions--quirks)
- [NuGet Packages](#nuget-packages)

---

## Tech Stack

| Technology | Version |
|---|---|
| .NET | 8.0 |
| ASP.NET Core | 8.0 |
| Entity Framework Core | 8.0.6 |
| MediatR | 12.3.0 |
| AutoMapper | 13.0.1 |
| FluentValidation | 11.9.2 |
| SQL Server | — |
| Swashbuckle (Swagger) | 6.7.0 |

---

## Solution Architecture

The solution follows the **Clean Architecture** (Onion) pattern with strict dependency inversion. Dependencies flow inward:

```
SchoolProject.Api  ──►  SchoolProject.Core  ──►  SchoolProject.Service  ──►  SchoolProject.Infrastructure  ──►  SchoolProject.Data
                                   │                                                │
                                   └── (MediatR commands/queries pipeline) ──────────┘
```

**Dependency rule**: Inner layers have no knowledge of outer layers. Data is the innermost, independent layer.

However, the actual `.csproj` references are:

| Project | References |
|---|---|
| **Data** | *(none)* |
| **Infrastructure** | Data |
| **Service** | Data, Infrastructure |
| **Core** | Data, Service |
| **Api** | Core |

> **Note**: `Core.csproj` references `Service.csproj` directly, relaxing strict onion purity for practical convenience.

---

## Project Structure

```
EduHub/
├── .github/workflows/dotnet.yml          # CI build workflow
├── SchoolProject/
│   ├── SchoolProject.sln
│   ├── SchoolProject.Data/               # Domain layer
│   │   ├── AppMetaData/Router.cs         # Route constant definitions
│   │   ├── Commons/
│   │   │   ├── LocalizableEntity.cs      # Base with NameAr/NameEn + GetLocalized()
│   │   │   └── GeneralLocalizableEntity.cs # Base with Localize(textAr, textEn)
│   │   ├── Entities/
│   │   │   ├── Student.cs                # Core entity — linked to Department
│   │   │   ├── Department.cs             # Department with Instructor manager
│   │   │   ├── Subject.cs                # Course/subject entity
│   │   │   ├── Instructor.cs             # Instructor with self-referencing supervisor
│   │   │   ├── StudentSubject.cs         # Many-to-many: Student ↔ Subject
│   │   │   ├── DepartmentSubject.cs      # Many-to-many: Department ↔ Subject
│   │   │   └── InstructorSubject.cs      # Many-to-many: Instructor ↔ Subject
│   │   └── Helpers/
│   │       └── StudentOrderingEnum.cs    # Sorting enum for paginated queries
│   │
│   ├── SchoolProject.Infrastructure/     # Persistence layer
│   │   ├── Context/ApplicationDbContext.cs  # EF Core DbContext
│   │   ├── InfrastructureBases/
│   │   │   ├── IGenericRepositoryAsync.cs  # Generic CRUD interface
│   │   │   └── GenericRepositoryAsync.cs   # Generic CRUD implementation
│   │   ├── Abstracts/IStudentRepository.cs # Student-specific repository contract
│   │   ├── Repositories/StudentRepository.cs  # Student repository impl
│   │   ├── Migrations/                    # EF Core migrations (5 sets)
│   │   └── ModuleInfrastructureDependencies.cs  # DI registration extension
│   │
│   ├── SchoolProject.Service/            # Business logic layer
│   │   ├── Abstracts/IStudentService.cs  # Student service contract
│   │   ├── Implementations/StudentService.cs  # Student service impl
│   │   └── ModuleServiceDependencies.cs  # DI registration extension
│   │
│   ├── SchoolProject.Core/              # Application / CQRS layer
│   │   ├── Bases/
│   │   │   ├── Response.cs              # Generic response wrapper
│   │   │   └── ResponseHandler.cs       # Factory for typed API responses
│   │   ├── Behaviors/ValidationBehavior.cs  # FluentValidation pipeline
│   │   ├── MiddleWare/ErrorHandlerMiddleware.cs  # Global exception handler
│   │   ├── Wrappers/
│   │   │   ├── PaginationResult.cs      # Paginated response wrapper
│   │   │   └── QueryableExtensions.cs   # Pagination extension method
│   │   ├── Resources/
│   │   │   ├── SharedResources.cs       # Localization key class
│   │   │   ├── SharedResourcesKeys.cs   # Constant string keys
│   │   │   ├── SharedResources.en.resx  # English translations
│   │   │   └── SharedResources.ar.resx  # Arabic translations
│   │   ├── Features/Students/
│   │   │   ├── Commands/
│   │   │   │   ├── Models/              # Create, Update, Delete commands
│   │   │   │   ├── Handlers/            # Command handlers
│   │   │   │   └── Validations/         # FluentValidation validators
│   │   │   └── Queries/
│   │   │       ├── Models/              # GetList, GetByID, Paginated queries
│   │   │       ├── Handlers/            # Query handlers
│   │   │       └── Responses/           # Response DTOs
│   │   ├── Mapping/StudentMapper/       # AutoMapper profiles
│   │   └── ModuleCoreDependencies.cs    # DI registration (MediatR, AutoMapper, FluentValidation)
│   │
│   └── SchoolProject.Api/               # API entry point
│       ├── Program.cs                   # Host builder, DI, middleware pipeline
│       ├── Controllers/StudentController.cs  # REST endpoints
│       ├── Base/AppControllerBase.cs    # Base controller with NewResult<T>()
│       ├── appsettings.json             # DB connection strings
│       ├── appsettings.Development.json
│       └── Properties/launchSettings.json  # Dev server config
```

---

## Layer Responsibilities

### Data Layer (SchoolProject.Data)
- **Entities**: `Student`, `Department`, `Subject`, `Instructor`, and join tables (`StudentSubject`, `DepartmentSubject`, `InstructorSubject`)
- **Base types**: `LocalizableEntity` (has `NameAr`/`NameEn` with `GetLocalized()`), `GeneralLocalizableEntity` (has `Localize(textAr, textEn)` method)
- **Enums**: `StudentOrderingEnum` for paginated query ordering
- **Route constants**: Centralized in `Router` static class (`Data/AppMetaData/Router.cs`)

### Infrastructure Layer (SchoolProject.Infrastructure)
- **DbContext**: `ApplicationDbContext` with 7 `DbSet<>` properties and composite key + relationship configuration in `OnModelCreating`
- **Generic Repository**: `IGenericRepositoryAsync<T>` / `GenericRepositoryAsync<T>` — CRUD operations, transaction support, tracking/no-tracking queries
- **Student Repository**: `StudentRepository` extends `GenericRepositoryAsync<Student>` adding `GetStudentsAsync()` with Department include
- **Migrations**: 5 migrations evolving the schema from initial creation to entity configuration

### Service Layer (SchoolProject.Service)
- Business logic between API handlers and repositories
- Contains `GetStudentsQueryable()`, `FilterStudentPaginatedQuery()` for paginated/filtered queries
- Name uniqueness checks: `IsNameExists()`, `IsNameExistsExcludeSelf()`
- Currently only `IStudentService`/`StudentService` implemented

### Core Layer (SchoolProject.Core)
- **CQRS**: Commands (`CreateStudentCommand`, `UpdateStudentCommand`, `DeleteStudentCommand`) and Queries (`GetStudentsListQuery`, `GetStudentByIDQuery`, `GetStudentPaginatedListQuery`) via MediatR
- **Handlers**: `StudentCommandHandler` and `StudentQueryHandler` — both extend `ResponseHandler` for standardized responses
- **Validators**: FluentValidation validators using `MustAsync` for database-driven uniqueness checks
- **Pipeline**: `ValidationBehavior<TRequest, TResponse>` intercepts all requests to run validators before handlers
- **AutoMapper**: `StudentProfile` (partial class) with query mappings and command mappings
- **Error Handling**: Global `ErrorHandlerMiddleware` catches exceptions and returns structured JSON responses
- **Localization**: `SharedResources` with .resx files for multilingual response messages

### API Layer (SchoolProject.Api)
- **Controllers**: `StudentController` extends `AppControllerBase` (which provides `NewResult<T>()` for status-code-aware responses)
- **Dependency Injection**: Registered in order: Infrastructure → Service → Core
- **Localization**: Configured with 4 cultures (en-US, fr-FR, de-DE, ar-EG), default is ar-EG
- **Swagger**: Enabled in Development and Production environments

---

## CQRS Request Flow

```
HTTP Request
    │
    ▼
StudentController (Api)
    │  _mediator.Send(command/query)
    ▼
ValidationBehavior (Core)
    │  Runs FluentValidation validators → throws ValidationException on failure
    ▼
Command/Query Handler (Core)
    │  Uses AutoMapper + IStudentService
    ▼
StudentService (Service)
    │  Business logic, name checks
    ▼
StudentRepository / GenericRepositoryAsync (Infrastructure)
    │  EF Core queries
    ▼
ApplicationDbContext → SQL Server
```

**Response flow** (reversed):
```
Handler → Response<T> / PaginationResult<T> → Controller → NewResult<T>() → HTTP response
```

---

## Database

- **Provider**: SQL Server (via `Microsoft.EntityFrameworkCore.SqlServer`)
- **Connection strings**: `appsettings.json` has three — `LocalConnection` (localdb), `RemoteConnection`, `NewRemoteConnection` (active/current remote DB)
- **Current schema** (after 5 migrations):
  - `Students` — with FK to Departments
  - `Departments` — with FK to Instructors (manager)
  - `Subjects` — courses with Arabic/English names and Period
  - `Instructors` — with self-referencing Supervisor FK
  - `StudentSubjects` — composite PK (StudID, SubID), with Grade
  - `DepartmentSubjects` — composite PK (DeptID, SubID)
  - `InstructorSubjects` — composite PK (InstructorID, SubjectID)

### Migration Commands

```bash
# Add a new migration
dotnet ef migrations add <Name> --project SchoolProject.Infrastructure --startup-project SchoolProject.Api

# Apply migrations to database
dotnet ef database update --project SchoolProject.Infrastructure --startup-project SchoolProject.Api
```

---

## Localization

The API supports multilingual responses via .NET's built-in localization:

- **Supported cultures**: `en-US`, `fr-FR`, `de-DE`, `ar-EG` (default)
- **Entity localization**: Entities inherit `GeneralLocalizableEntity` which provides `Localize(textAr, textEn)` — returns Arabic text when thread culture starts with "ar", English otherwise
- **Response localization**: `ResponseHandler` uses `IStringLocalizer<SharedResources>` to translate response messages
- **Validation localization**: Validators reference localized strings via `SharedResourcesKeys` constants
- **Resource files**: `Core/Resources/SharedResources.{culture}.resx`

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server instance (local or remote)

### Setup

```bash
# Clone and restore
git clone <repo-url>
cd EduHub
dotnet restore SchoolProject/SchoolProject.sln

# Build
dotnet build SchoolProject/SchoolProject.sln

# Update connection string in appsettings.json, then apply migrations
dotnet ef database update --project SchoolProject.Infrastructure --startup-project SchoolProject.Api

# Run
dotnet run --project SchoolProject/SchoolProject.Api
```

The API launches on `http://localhost:5059` (HTTPS: `https://localhost:7132`) with Swagger at `/swagger`.

---

## API Endpoints

Routes defined in `Data/AppMetaData/Router.cs`:

### Student Endpoints

| Method | Route | Description |
|---|---|---|
| GET | `/Api/V1/Student/List` | Get all students |
| GET | `/Api/V1/Student/{id}` | Get student by ID |
| GET | `/Api/V1/Student/Paginated` | Get paginated student list |
| POST | `/Api/V1/Student/Create` | Create a new student |
| PUT | `/Api/V1/Student/Update` | Update an existing student |
| DELETE | `/Api/V1/Student/Delete{StudID}` | Delete a student |

All responses are wrapped in `Response<T>` with `Succeeded`, `Message`, `Data`, `Errors`, `StatusCode` fields. Paginated responses use `PaginationResult<T>` with `CurrentPage`, `TotalPage`, `TotalCount`, `PageSize`.

---

## Key Design Decisions & Quirks

1. **Relaxed Onion Layering**: `Core.csproj` references `Service.csproj` directly (not just Data), deviating from strict Clean Architecture. This allows handlers to call service methods without a separate abstraction layer between Core and Service.

2. **Generic Repository with Transaction Support**: `GenericRepositoryAsync<T>` includes `BeginTransaction()`, `Commit()`, `Rollback()` for manual transaction control. The `StudentService.DeleteStudentAsync()` wraps delete in a try/catch with manual rollback.

3. **DeleteStudentValidator Bug**: `DeleteStudentValidator` has a **parameterless constructor** that calls `ApplyCustomValidation()` before `_studentService` is injected. This causes a `NullReferenceException` at runtime. The parameterized constructor (which receives DI) is correct but unused by FluentValidation's auto-discovery if the parameterless one exists.

4. **Localization via Thread Culture**: Entity localization uses `Thread.CurrentThread.CurrentCulture`, which depends on the `RequestLocalizationMiddleware` setting the thread culture per-request. This works but differs from the typical `IStringLocalizer<T>` pattern used for response messages.

5. **Swagger in Production**: `Program.cs` enables `UseSwaggerUI()` twice in Production (lines 68-69), which is likely unintentional.

6. **Live DB Credentials in appsettings.json**: The file contains connection strings for a remote database. Do **not** commit changes to this file.

7. **No Authentication/Authorization**: The API has no Identity middleware or JWT auth — endpoints are currently unprotected.

8. **No Test Project**: The solution has no test project; CI workflow (`dotnet.yml`) runs `dotnet test` but there are no tests to execute.

---

## NuGet Packages

### SchoolProject.Api
| Package | Version |
|---|---|
| `Microsoft.EntityFrameworkCore` | 8.0.6 |
| `Microsoft.EntityFrameworkCore.Design` | 8.0.6 |
| `Microsoft.EntityFrameworkCore.SqlServer` | 8.0.6 |
| `Microsoft.EntityFrameworkCore.Tools` | 8.0.6 |
| `Microsoft.Extensions.DependencyInjection` | 8.0.0 |
| `Swashbuckle.AspNetCore.Swagger` | 6.7.0 |
| `Swashbuckle.AspNetCore.SwaggerGen` | 6.7.0 |
| `Swashbuckle.AspNetCore.SwaggerUI` | 6.7.0 |

### SchoolProject.Core
| Package | Version |
|---|---|
| `AutoMapper` | 13.0.1 |
| `FluentValidation` | 11.9.2 |
| `FluentValidation.DependencyInjectionExtensions` | 11.9.2 |
| `MediatR` | 12.3.0 |
| `Microsoft.AspNetCore.Http.Abstractions` | 2.2.0 |
| `Microsoft.Extensions.Localization.Abstractions` | 8.0.7 |

### SchoolProject.Infrastructure
| Package | Version |
|---|---|
| `Microsoft.EntityFrameworkCore` | 8.0.6 |
| `Microsoft.EntityFrameworkCore.Relational` | 8.0.6 |
| `Microsoft.EntityFrameworkCore.SqlServer` | 8.0.6 |
| `Microsoft.EntityFrameworkCore.Tools` | 8.0.6 |

---

## CI/CD

A GitHub Actions workflow (`.github/workflows/dotnet.yml`) runs on push/PR to `main`:

1. Checkout code
2. Setup .NET 8.0.x
3. `dotnet restore`
4. `dotnet build --no-restore`
5. `dotnet test --no-build --verbosity normal`

---

## Entity Relationship Diagram

```
┌──────────────┐     ┌──────────────────┐     ┌──────────────┐
│   Student    │     │ StudentSubject   │     │   Subject    │
├──────────────┤     ├──────────────────┤     ├──────────────┤
│ StudID (PK)  │◄───►│ StudID (PK, FK) │────►│ SubID (PK)   │
│ NameEn       │     │ SubID (PK, FK)  │     │ SubjectNameAr│
│ NameAr       │     │ Grade            │     │ SubjectNameEn│
│ Address      │     └──────────────────┘     │ Period       │
│ Phone        │                               └──────┬───────┘
│ DepartmentID │                                      │
└──────┬───────┘                              ┌───────┴────────┐
       │                                      │                │
       ▼                           ┌──────────┴──────┐  ┌──────┴───────────┐
┌──────────────┐                   │ DeptSubject     │  │ InstructorSubject│
│ Department   │                   ├─────────────────┤  ├──────────────────┤
├──────────────┤                   │ DeptID (PK,FK)  │  │ InstructorID(PK) │
│ DeptID (PK)  │──────────────────►│ SubID (PK,FK)   │  │ SubjectID (PK)   │
│ NameEn       │                   └─────────────────┘  └──────────────────┘
│ NameAr       │                                              │
│InstManager(FK)──┐                                          │
└────────────────┘│                                   ┌──────┴───────┐
                  │                                   │  Instructor  │
                  └────►┌──────────────┐              ├──────────────┤
                        │  Instructor  │◄─────────────┤ InstID (PK)  │
                        ├──────────────┤   SupervisorID│ ENameAr      │
                        │ DeptID (FK)  │              │ ENameEn      │
                        │ SupervisorID │              │ Address      │
                        └──────────────┘              │ Position     │
                                                      │ Salary       │
                                                      │ DepartmentID │
                                                      └──────────────┘
```

---

## License

This project is for educational/demonstration purposes.
