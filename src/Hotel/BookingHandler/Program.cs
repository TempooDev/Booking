using Booking.Hotel.BookingHandler;

var builder = Host.CreateApplicationBuilder(args);
builder.AddServiceDefaults();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(BookingEventProcessor.ActivitySourceName));

// Añadir configuración por defecto de Aspire
// Configurar logging
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
    logging.AddFilter("Azure.Messaging.EventHubs", LogLevel.Debug);
});

// Registrar el worker como servicio hosteado
builder.Services.AddHostedService<BookingEventProcessor>();

var host = builder.Build();
await host.RunAsync();
