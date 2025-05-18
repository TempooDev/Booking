using Booking.Core.Common.Infrastructure.Persistence;
using Booking.Core.Common.Infrastructure.Services;
using Booking.Core.Users.Domain.Repositories;
using Booking.Core.Users.Infrastructure.Repositories;

using FluentValidation;

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Shared.Common.Behaviours;
using Shared.Common.Infrastructure.Services;
using Shared.Common.Interfaces;

namespace Booking.Core.Common;

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

        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }

    public static WebApplicationBuilder AddMessaging(this WebApplicationBuilder builder)
    {
        builder.AddAzureServiceBusClient(connectionName: "hotel");

        return builder;
    }

    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("booking-db"), b =>
                b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

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