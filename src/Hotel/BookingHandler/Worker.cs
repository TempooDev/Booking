using System.Diagnostics;
using System.Text.Json;

using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using Azure.Storage.Blobs;

namespace Booking.Hotel.BookingHandler;

public class BookingEventProcessor : BackgroundService
{
    public const string ActivitySourceName = "ManageBooking";
    private readonly ILogger<BookingEventProcessor> _logger;
    private readonly EventProcessorClient _processor;
    private readonly BlobContainerClient _storageClient;
    private static readonly ActivitySource ActivitySource = new(ActivitySourceName);

    public BookingEventProcessor(
        ILogger<BookingEventProcessor> logger,
        IConfiguration configuration)
    {
        _logger = logger;

        try
        {
            // Configuración del procesador de eventos
            var ehubNamespace = configuration.GetConnectionString("eventhubns")
                ?? "Endpoint=sb://localhost;SharedAccessKeyName=admin;SharedAccessKey=admin;EntityPath=booking";

            _logger.LogInformation("Usando Event Hub connection string: {ConnectionString}", ehubNamespace);

            // Usar Azure Storage Emulator para desarrollo local
            var storageConnString = configuration.GetConnectionString("storage");
            var blobContainerName = "booking-events-checkpoint";

            _storageClient = new BlobContainerClient(storageConnString, blobContainerName);
            _logger.LogInformation("Creando Event Processor Client...");
            _logger.LogInformation("Creando Event Processor Client...");

            // Crear el procesador de eventos
            _processor = new EventProcessorClient(
                _storageClient,
                EventHubConsumerClient.DefaultConsumerGroupName,
                ehubNamespace,
                "booking");

            // Registrar handlers
            _processor.ProcessEventAsync += ProcessEventHandler;
            _processor.ProcessErrorAsync += ProcessErrorHandler;

            _logger.LogInformation("Event Processor Client creado exitosamente");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al inicializar el Event Processor Client");
            throw;
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deteniendo procesador de eventos de Hotel Service...");
        await _processor.StopProcessingAsync(cancellationToken);
        await base.StopAsync(cancellationToken);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Iniciando procesador de eventos de Booking en Hotel Service...");
            if (_storageClient != null)
            {
                if (_storageClient != null)
                {
                    await _storageClient.CreateIfNotExistsAsync(cancellationToken: stoppingToken);
                    _logger.LogInformation("Contenedor de checkpoint creado/verificado");
                }
            }

            await _processor.StartProcessingAsync(stoppingToken);
            _logger.LogInformation("Procesador de eventos iniciado");

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error en el procesador de eventos de Hotel Service");
            throw;
        }
    }

    private async Task ProcessEventHandler(ProcessEventArgs args)
    {
        try
        {
            if (args.Data != null)
            {
                var eventBody = args.Data.EventBody.ToString();
                _logger.LogInformation("Procesando evento: {EventBody}", eventBody);

                // Generar un nombre único para el blob
                var blobName = $"{DateTime.UtcNow:yyyy/MM/dd/HH-mm-ss}-{Guid.NewGuid()}.json";

                var blobClient = _storageClient.GetBlobClient(blobName);

                // Crear un objeto con metadatos adicionales
                var eventData = new
                {
                    Timestamp = DateTime.UtcNow,
                    EventBody = eventBody,
                    PartitionId = args.Partition.PartitionId,
                    Offset = args.Data.Offset,
                    SequenceNumber = args.Data.SequenceNumber,
                };

                // Convertir a JSON
                var jsonContent = JsonSerializer.Serialize(
                    eventData);

                // Subir el contenido JSON al blob
                using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonContent)))
                {
                    await blobClient.UploadAsync(stream, overwrite: true);
                }

                // Marcar el evento como procesado
                await args.UpdateCheckpointAsync(args.CancellationToken);
                _logger.LogInformation("Evento procesado y checkpoint actualizado");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error procesando evento en Hotel Service");
            throw;
        }
    }

    private Task ProcessErrorHandler(ProcessErrorEventArgs args)
    {
        _logger.LogError(args.Exception, "Error en el procesador de eventos de Hotel: {ErrorMessage}", args.Exception.Message);
        return Task.CompletedTask;
    }
}