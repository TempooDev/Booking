using System.Diagnostics;
using System.Text.Json;

using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;

namespace Booking.Hotel.BookingHandler;

public class BookingEventProcessor : BackgroundService
{
    public const string ActivitySourceName = "ManageBooking";
    private readonly ILogger<BookingEventProcessor> _logger;
    private readonly ServiceBusProcessor _processor;
    private static readonly ActivitySource ActivitySource = new(ActivitySourceName);
    private readonly BlobContainerClient _blobContainerClient;

    public BookingEventProcessor(
        ILogger<BookingEventProcessor> logger,
        ServiceBusClient serviceBusClient,
        BlobContainerClient blobContainerClient)
    {
        _logger = logger;

        try
        {
            var topicName = "booking";
            _processor = serviceBusClient.CreateProcessor(topicName, new ServiceBusProcessorOptions());

            // Registrar handlers
            _processor.ProcessMessageAsync += ProcessMessageHandler;
            _processor.ProcessErrorAsync += ProcessErrorHandler;

            _logger.LogInformation("Service Bus Processor creado exitosamente");

            _blobContainerClient = blobContainerClient;
            _blobContainerClient.CreateIfNotExists();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al inicializar el Service Bus Processor");
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

            // start processing
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

    private async Task ProcessMessageHandler(ProcessMessageEventArgs args)
    {
        try
        {
            string body = args.Message.Body.ToString();
            _logger.LogInformation("Procesando mensaje: {Body}", body);

            // Crear un nombre de blob único
            string blobName = $"{DateTime.UtcNow:yyyy-MM-dd_HH-mm-ss}_{Guid.NewGuid()}.json";
            BlobClient blobClient = _blobContainerClient.GetBlobClient(blobName);

            // Serializar el mensaje a JSON
            string jsonMessage = JsonSerializer.Serialize(new { Body = body });

            // Subir el mensaje al blob storage
            using (MemoryStream stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(jsonMessage)))
            {
                await blobClient.UploadAsync(stream, overwrite: true);
            }

            _logger.LogInformation("Mensaje guardado en Blob Storage: {blobName}", blobName);

            // Confirmar el mensaje
            await args.CompleteMessageAsync(args.Message);
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
