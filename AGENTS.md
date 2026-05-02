# AGENTS.md — EduHub

## Build & Run

```bash
dotnet restore SchoolProject/SchoolProject.sln
dotnet build SchoolProject/SchoolProject.sln
dotnet run --project SchoolProject/SchoolProject.Api
```

Uses .NET 8.0 (`net8.0`). No test project exists yet.

## Architecture (Clean Architecture / Onion)

Solution has 5 projects layered by dependency direction:

```
Api → Core → Service → Infrastructure → Data
           ↕ (MediatR)
```

- **SchoolProject.Data** — entity classes, enums, shared base types (independent, no deps)
- **SchoolProject.Infrastructure** — EF Core `DbContext`, migrations, generic repository pattern, concrete repositories. Depends on Data
- **SchoolProject.Service** — business logic interfaces + implementations. Depends on Data + Infrastructure
- **SchoolProject.Core** — CQRS via MediatR (Commands, Queries, Handlers), AutoMapper profiles, FluentValidation validators, error middleware, localized response wrappers. Depends on Data + Service
- **SchoolProject.Api** — ASP.NET Core Web API entrypoint. Registers all DI modules, localization, middleware. Depends on Core only (transitively gets everything)

Key quirks:
- The `.sln` is in `SchoolProject/` subdirectory, not repo root
- DI is wired via static extension methods: `AddInfrastructureDependencies()`, `AddServiceDependencies()`, `AddCoreDependencies()` — called in that order in `Program.cs`
- `Core.csproj` references both Data and Service (not just Data), which means the "strict" onion layering is relaxed — Core can call Service directly

## DB & Migrations

- EF Core + SQL Server
- Connection strings in `SchoolProject.Api/appsettings.json` (key: `NewRemoteConnection` for remote, `LocalConnection` for local)
- Migrations in `SchoolProject.Infrastructure/Migrations/`, managed from `SchoolProject.Api` (it holds the EF Design tools)
- Add migration: `dotnet ef migrations add <Name> --project SchoolProject.Infrastructure --startup-project SchoolProject.Api`
- Apply: `dotnet ef database update --project SchoolProject.Infrastructure --startup-project SchoolProject.Api`

## CQRS Pattern

All request flow: Controller → MediatR → Handler → Service → Repository → EF

- **Commands**: `Core/Features/{Feature}/Commands/Models/` (IRequest<>), `Handlers/`, `Validations/`
- **Queries**: `Core/Features/{Feature}/Queries/Models/`, `Handlers/`, `Responses/`
- Only Student feature exists currently
- Route constants are centralized in `Data/AppMetaData/Router.cs`

## Localization

- Supported cultures: en-US, fr-FR, de-DE, ar-EG (default)
- .resx files in `Core/Resources/` with `SharedResources` key class
- Entities use `LocalizableEntity`/`GeneralLocalizableEntity` base classes with `Localize()` method switching on current thread culture
- Response messages go through `IStringLocalizer<SharedResources>`

## Key Notes

- `appsettings.json` contains live remote DB credentials — do not commit changes to it
- The Infrastructure layer has `IGenericRepositoryAsync<T>` (CRUD base) and `GenericRepositoryAsync<T>` (primary constructor with `ApplicationDbContext`)
- Student's `DeleteStudentValidator` has a bug: parameterless constructor bypasses service injection, calls `ApplyCustomValidation()` before `_studentService` is set → will NPE at runtime
- No Identity/Auth middleware yet
- Swagger enabled in all environments (Dev shows SwaggerUI; Production also shows SwaggerUI via duplicate calls — likely unintentional)
