using System;
using System.Threading.Tasks;

using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Hotel.EventConsumer
{
    public class BookingHandler
    {
        private readonly ILogger<BookingHandler> _logger;
        private readonly BlobServiceClient _blobServiceClient;

        public BookingHandler(ILogger<BookingHandler> logger, BlobServiceClient blobServiceClient)
        {
            _logger = logger;
            _blobServiceClient = blobServiceClient;
        }

        [Function(nameof(BookingHandler))]
        public async Task Run(
        [ServiceBusTrigger(topicName: "booking", subscriptionName: "hotel", Connection = "hotel")]
        ServiceBusReceivedMessage message,
        ServiceBusMessageActions messageActions)
        {
            _logger.LogInformation("Message ID: {id}", message.MessageId);
            _logger.LogInformation("Message Body: {body}", message.Body);
            _logger.LogInformation("Message Content-Type: {contentType}", message.ContentType);

            // Store the message content in Blob Storage
            var containerClient = _blobServiceClient.GetBlobContainerClient("blobs");

            // Ensure the container exists
            await containerClient.CreateIfNotExistsAsync();

            var blobClient = containerClient.GetBlobClient($"{message.MessageId}.json");
            await blobClient.UploadAsync(new BinaryData(message.Body));

            // Complete the message
            await messageActions.CompleteMessageAsync(message);
        }
    }
}
