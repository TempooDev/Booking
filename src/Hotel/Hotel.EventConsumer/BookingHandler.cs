using System;
using System.Text.Json;
using System.Threading.Tasks;

using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Hotel.BookingHandler
{
    public class BookingHandler
    {
        private readonly ILogger<BookingHandler> _logger;
        private readonly BlobContainerClient _blobContainerClient;
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };

        public BookingHandler(ILogger<BookingHandler> logger, BlobServiceClient blobServiceClient)
        {
            _logger = logger;
            _blobContainerClient = blobServiceClient.GetBlobContainerClient("booking-blobs");

            // Crear contenedor si no existe (solo una vez al inicio)
            _blobContainerClient.CreateIfNotExists(PublicAccessType.None);
        }

        [Function("BookingHandler")]
        public async Task Run(
            [ServiceBusTrigger("booking", "hotel", Connection = "hotel")]
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions)
        {
            _logger.LogInformation("Processing message ID: {MessageId}", message.MessageId);

            try
            {
                // 1. Validar el mensaje
                if (!ValidateMessage(message))
                {
                    await messageActions.DeadLetterMessageAsync(
                        message: message,
                        deadLetterReason: "InvalidMessage",
                        deadLetterErrorDescription: "El mensaje no contiene datos válidos");
                    return;
                }

                // 2. Procesar y subir a Blob Storage
                var (blobName, uploadOptions) = PrepareBlobUpload(message);
                await UploadToBlobStorage(message.Body, blobName, uploadOptions);

                // 3. Completar el mensaje
                await messageActions.CompleteMessageAsync(message);
                _logger.LogInformation("Message processed successfully. Blob: {BlobName}", blobName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message {MessageId}", message.MessageId);
                await HandleFailedMessage(message, messageActions, ex);
            }
        }

        private bool ValidateMessage(ServiceBusReceivedMessage message)
        {
            if (message.Body == null || message.Body.ToMemory().Length == 0)
            {
                _logger.LogWarning("Empty message body received");
                return false;
            }

            // Opcional: Validar schema JSON
            try
            {
                JsonDocument.Parse(message.Body.ToMemory());
                return true;
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Invalid JSON format");
                return false;
            }
        }

        private static (string BlobName, BlobUploadOptions UploadOptions) PrepareBlobUpload(ServiceBusReceivedMessage message)
        {
            // Estructura de carpetas por fecha
            var blobName = $"bookings/{message.Subject}/{DateTime.UtcNow:yyyy/MM/dd}/{message.MessageId}.json";

            var metadata = new BlobMetadata
            {
                MessageId = message.MessageId,
                ContentType = message.ContentType ?? "application/json",
                EnqueuedTime = message.EnqueuedTime.UtcDateTime.ToString("O"),
                CorrelationId = message.CorrelationId ?? string.Empty,
            };

            var uploadOptions = new BlobUploadOptions
            {
                Metadata = metadata.ToDictionary(),
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = "application/json",
                    ContentEncoding = "utf-8",
                },
            };

            return (blobName, uploadOptions);
        }

        private async Task UploadToBlobStorage(BinaryData messageBody, string blobName, BlobUploadOptions uploadOptions)
        {
            var blobClient = _blobContainerClient.GetBlobClient(blobName);

            using (JsonDocument jsonDoc = JsonDocument.Parse(messageBody.ToMemory()))
            using (var memoryStream = new MemoryStream())
            {
                await JsonSerializer.SerializeAsync(
                    memoryStream, jsonDoc, _jsonOptions);
                memoryStream.Position = 0;
                await blobClient.UploadAsync(memoryStream, uploadOptions);
            }
        }

        private async Task HandleFailedMessage(
            ServiceBusReceivedMessage message,
            ServiceBusMessageActions messageActions,
            Exception ex)
        {
            try
            {
                await messageActions.DeadLetterMessageAsync(
                    message: message,
                    deadLetterReason: "ProcessingError",
                    deadLetterErrorDescription: $"{ex.GetType().Name}: {ex.Message}");
            }
            catch (Exception dlEx)
            {
                _logger.LogCritical(dlEx, "Failed to dead-letter message {MessageId}", message.MessageId);
            }
        }
    }

    // Clase auxiliar para metadatos
    public class BlobMetadata
    {
        public required string MessageId { get; set; }
        public required string ContentType { get; set; }
        public required string EnqueuedTime { get; set; }
        public required string CorrelationId { get; set; }

        public System.Collections.Generic.Dictionary<string, string> ToDictionary() =>
            new System.Collections.Generic.Dictionary<string, string>
            {
                [nameof(MessageId)] = MessageId,
                [nameof(ContentType)] = ContentType,
                [nameof(EnqueuedTime)] = EnqueuedTime,
                [nameof(CorrelationId)] = CorrelationId,
            };
    }
}