using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

using Shared.Common;
using Shared.Common.Interfaces;

namespace Booking.Booking.Application.Common.Infrastructure.Persistence;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // Configuración apuntando al directorio de la API
        var apiPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Api");

        var configuration = new ConfigurationBuilder()
            .SetBasePath(apiPath)
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseNpgsql(
            configuration.GetConnectionString("booking"),
            opt => opt.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName));

        // Servicios mock para tiempo de diseño
        var currentUserService = new DesignTimeCurrentUserService();
        var domainEventService = new DesignTimeDomainEventService();
        var dateTime = new DesignTimeDateTimeService();

        return new ApplicationDbContext(
            optionsBuilder.Options,
            currentUserService,
            domainEventService,
            dateTime);
    }
}

// Implementaciones mock para tiempo de diseño
internal class DesignTimeCurrentUserService : ICurrentUserService
{
    public string? UserId => "DESIGN-TIME-USER";
}

internal class DesignTimeDomainEventService : IDomainEventService
{
    public Task Publish(DomainEvent domainEvent) => Task.CompletedTask;
}

internal class DesignTimeDateTimeService : IDateTime
{
    public DateTime Now => DateTime.UtcNow;
}