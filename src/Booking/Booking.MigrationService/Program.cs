using Booking.Booking.Application.Common;
using Booking.Booking.Application.Common.Infrastructure.Services;
using Booking.Booking.MigrationService;
using Booking.Booking.MigrationService.Services;
using Booking.Shared.Application.Common.Infrastructure.Services;

using Microsoft.AspNetCore.Http;

using Shared.Common.Interfaces;

var builder = Host.CreateApplicationBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(Worker.ActivitySourceName));

// Registrar la implementación NoOp del servicio de eventos
builder.Services.AddSingleton<IDomainEventService, NoOpDomainEventService>();

// Usar el método específico para migraciones
builder.Services.AddMigrationServices(builder.Configuration);

// Servicios básicos necesarios
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();