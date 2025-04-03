using Booking.Booking.Application.Common;
using Booking.Booking.Application.Common.Infrastructure.Services;
using Booking.Booking.MigrationService;
using Booking.Shared.Application.Common.Infrastructure.Services;

using Microsoft.AspNetCore.Http;

using Shared.Common.Interfaces;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddPersistence(builder.Configuration);

// Register IHttpContextAccessor
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

builder.Services.AddSingleton<IDomainEventService, DomainEventService>();
builder.Services.AddSingleton<IDateTime, DateTimeService>();
builder.Services.AddSingleton<ICurrentUserService, CurrentUserService>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();