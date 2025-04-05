using Booking.Hotel.BookingHandler;

using Microsoft.Extensions.Azure;

var builder = Host.CreateApplicationBuilder(args);
builder.AddServiceDefaults();

builder.Services.AddOpenTelemetry()
    .WithTracing(tracing => tracing.AddSource(BookingEventProcessor.ActivitySourceName));

// Configurar logging
builder.Services.AddLogging();

builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(builder.Configuration.GetConnectionString("booking-blobs"));
    clientBuilder.AddServiceBusClient(builder.Configuration.GetConnectionString("servicebus"));
});

// Add BlobContainerClient registration
builder.Services.AddSingleton(sp =>
{
    var blobServiceClient = sp.GetRequiredService<Azure.Storage.Blobs.BlobServiceClient>();
    var containerName = "bookings-blobs"; // Replace with your actual container name
    return blobServiceClient.GetBlobContainerClient(containerName);
});

builder.Services.AddHostedService<BookingEventProcessor>();

var host = builder.Build();
await host.RunAsync();
