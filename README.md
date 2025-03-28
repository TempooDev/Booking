# Booking Microservices Project

A modern microservices architecture built with .NET 8, implementing Domain-Driven Design (DDD) principles and using .NET Aspire for cloud-native development.

## Architecture Overview

### Domain-Driven Design (DDD) Implementation

The project follows a clean DDD architecture with these layers:

```
src/
├── Booking/
│   ├── Application/              # Application layer
│   │   ├── Common/              # Shared components
│   │   │   ├── Infrastructure/  # Infrastructure implementations
│   │   │   └── Domain/         # Domain abstractions
│   │   ├── TodoItems/          # Feature module
│   │   │   ├── Domain/         # Domain model
│   │   │   └── ValueObjects/   # Value objects
│   │   └── TodoLists/          # Feature module
│   │       └── Domain/         # Domain model
│   ├── Api/                     # API layer
│   └── MigrationService/        # Database migrations
└── Aspire/                      # .NET Aspire orchestration
    └── Booking.AppHost/         # Application host
```

#### Layer Responsibilities

- **Domain Layer**: Contains business logic, entities, value objects, and domain events
- **Application Layer**: Orchestrates the domain objects to perform tasks
- **Infrastructure Layer**: Implements interfaces defined in the domain
- **API Layer**: Handles HTTP requests and responses
- **Migration Service**: Manages database schema evolution

### Value Objects Pattern

Example of a Value Object implementation:

```csharp
public record Colour
{
    private Colour(string code) => Code = code;
    public string Code { get; }
    
    public static ErrorOr<Colour> From(string code) => 
        // Validation logic
}
```

## Creating New Microservices

To add a new microservice:

1. Create a new folder under `src/`
2. Follow the DDD structure:
```
NewService/
├── Application/
│   ├── Common/
│   └── Features/
├── Api/
└── MigrationService/
```

3. Register in Aspire's `Program.cs`:

```csharp
var builder = DistributedApplication.CreateBuilder(args);

var newServiceDb = sqlServer.AddDatabase("newservice");
var newService = builder.AddProject<Projects.NewService>("new-service")
    .WithReference(newServiceDb);
```

## .NET Aspire Integration

Aspire provides:

- **Service Discovery**: Automatic service registration and discovery
- **Health Monitoring**: Built-in health checks
- **Configuration Management**: Centralized configuration
- **Container Orchestration**: Managed container lifecycle
- **Local Development**: Simplified local development experience

### Example Aspire Configuration

```csharp
var builder = DistributedApplication.CreateBuilder(args);

// Database
var sqlServer = builder.AddPostgres("sql")
    .WithLifetime(ContainerLifetime.Persistent);

// Migrations
var bookingDb = sqlServer.AddDatabase("booking");
var bookingMigration = builder.AddProject<Projects.Booking_MigrationService>()
    .WithReference(bookingDb);

// API
builder.AddProject<Projects.Booking_Api>()
    .WithReference(bookingDb)
    .WaitForCompletion(bookingMigration);
```

## Development Workflow

1. Clone the repository
2. Install dependencies:
```bash
dotnet restore
```

3. Run the project:
```bash
cd src/Aspire/Booking.AppHost
dotnet run
```

## Best Practices

- Keep domains bounded and well-defined
- Use Value Objects for domain concepts
- Implement domain events for cross-boundary communication
- Follow SOLID principles
- Use clean architecture patterns

## Testing

```bash
dotnet test
```

## Contributing

1. Fork the repository
2. Create your feature branch
3. Commit your changes
4. Push to the branch
5. Create a Pull Request

## Database Migrations

### Prerequisites

- EF Core CLI tools installed globally:
```bash
dotnet tool install --global dotnet-ef
```

### Migration Commands

#### Create Initial Migration
```bash
cd src/Booking/Application
dotnet ef migrations add InitialCreate -o Common/Infrastructure/Persistence/Migrations
```

#### Apply Migrations
```bash
dotnet ef database update
```

### Migration Structure

```
Application/
└── Common/
    └── Infrastructure/
        └── Persistence/
            ├── ApplicationDbContext.cs
            └── Migrations/
                ├── YYYYMMDDHHMMSS_InitialCreate.cs
                ├── YYYYMMDDHHMMSS_InitialCreate.Designer.cs
                └── ApplicationDbContextModelSnapshot.cs
```

### Useful Commands

#### List Migrations
```bash
dotnet ef migrations list
```

#### Remove Last Migration
```bash
dotnet ef migrations remove
```

#### Generate SQL Script
```bash
dotnet ef migrations script
```

### Migration Best Practices

1. **Naming Conventions**
   - Use descriptive names: `AddUserTable`, `UpdateProductSchema`
   - Avoid generic names: `Update1`, `Change2`

2. **Version Control**
   - Always include migrations in source control
   - Never modify existing migrations in production
   - Create new migrations for additional changes

3. **Testing**
   - Test migrations in development environment
   - Verify both up and down migrations
   - Include migration tests in CI pipeline

### Troubleshooting

#### Common Issues

1. **Context Not Found**
   - Verify correct namespace
   - Check DbContext registration in DI

2. **Connection Errors**
   - Verify PostgreSQL is running
   - Check connection string in `appsettings.json`
   - Ensure database exists

3. **Migration Conflicts**
   - Clean existing migrations:
```bash
rm -rf Common/Infrastructure/Persistence/Migrations
```
   - Recreate initial migration:
```bash
dotnet ef migrations add InitialCreate -o Common/Infrastructure/Persistence/Migrations
```

#### Required Packages

```xml
<ItemGroup>
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.0" />
    <PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.0" />
</ItemGroup>
```

### Migration Service Integration

The MigrationService project handles automatic database migrations during deployment:

```csharp
public class Worker : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        // Apply migrations on startup
        await using var scope = _serviceProvider.CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await context.Database.MigrateAsync(cancellationToken);
    }
}
```

This ensures:
- Automatic migration execution
- Safe deployment process
- Database schema consistency