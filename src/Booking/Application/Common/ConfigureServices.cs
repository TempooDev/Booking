using Booking.Booking.Application.Common.Infrastructure.Persistence;
using Booking.Booking.Application.Common.Infrastructure.Services;
using Booking.Shared.Application.Common.Infrastructure.Services;

using FluentValidation;

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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

        services.AddAzureClients(clientBuilder =>
            {
                clientBuilder.AddServiceBusClient(configuration.GetConnectionString("servicebus"));
            });
        return services;
    }

    public static WebApplicationBuilder AddMessaging(this WebApplicationBuilder builder)
    {
        builder.AddAzureServiceBusClient(connectionName: "serviceBus");

        return builder;
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