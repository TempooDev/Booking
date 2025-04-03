using Azure.Messaging.EventHubs.Producer;

using Booking.Application.Common.Interfaces;
using Booking.Booking.Application.Common.Infrastructure.Persistence;
using Booking.Booking.Application.Common.Infrastructure.Services;
using Booking.Shared.Application.Common.Infrastructure.Services;

using FluentValidation;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Shared.Common.Behaviours;
using Shared.Common.Interfaces;

namespace Booking.Booking.Application.Common;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(options =>
        {
            options.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);

            options.AddOpenBehavior(typeof(AuthorizationBehaviour<,>));
            options.AddOpenBehavior(typeof(PerformanceBehaviour<,>));
            options.AddOpenBehavior(typeof(ValidationBehaviour<,>));
        });

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);

        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IDomainEventService, DomainEventService>();

        services.AddTransient<IDateTime, DateTimeService>();

        services.AddSingleton<ICurrentUserService, CurrentUserService>();

        return services;
    }

    public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(sp =>
        {
            var connectionString = configuration.GetConnectionString("eventhubns")
                ?? throw new InvalidOperationException("No se encontró la cadena de conexión 'eventhubns'");

            return new EventHubProducerClient(connectionString, "booking");
        });

        return services;
    }

    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
              options.UseNpgsql(
                  configuration.GetConnectionString("booking-db"),
                  b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        return services;
    }

    public static IServiceCollection AddMigrationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Solo servicios necesarios para migración
        services.AddPersistence(configuration);
        services.AddTransient<IDateTime, DateTimeService>();
        services.AddSingleton<ICurrentUserService, CurrentUserService>();

        return services;
    }
}