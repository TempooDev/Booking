using Booking.Booking.Application.Common.Infrastructure.Persistence;
using Booking.Booking.Application.Common.Infrastructure.Services;
using Booking.Booking.MigrationService;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shared.Common.Interfaces;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseNpgsql(
                            builder.Configuration.GetConnectionString("booking"),
                            b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

// Register IHttpContextAccessor
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddScoped<IDomainEventService, DomainEventService>();
builder.Services.AddTransient<IDateTime, DateTimeService>();
builder.Services.AddSingleton<ICurrentUserService, CurrentUserService>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
