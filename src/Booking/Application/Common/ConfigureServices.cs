using Booking.Application.Common.Interfaces;
using Booking.Booking.Application.Common.Infrastructure.Persistence;
using Booking.Booking.Application.Common.Infrastructure.Services;
using Booking.Booking.Application.TodoItems.Infrastructure.Files;

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
        services.AddDbContext<ApplicationDbContext>(options =>
               options.UseNpgsql(
                   configuration.GetConnectionString("booking-db"),
                   b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        services.AddScoped<IDomainEventService, DomainEventService>();

        services.AddTransient<IDateTime, DateTimeService>();
        services.AddTransient<ICsvFileBuilder, CsvFileBuilder>();

        services.AddSingleton<ICurrentUserService, CurrentUserService>();

        return services;
    }
}