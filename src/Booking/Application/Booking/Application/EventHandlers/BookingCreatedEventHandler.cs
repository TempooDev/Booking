using System.Text.Json;

using Azure.Messaging.ServiceBus;

using Booking.Booking.Application.Booking.Domain.Events;

using MediatR;

using Microsoft.Extensions.Logging;

using Shared.Common.Models;

namespace Booking.Booking.Application.Booking.Application.EventHandlers;

internal sealed class BookingCreatedEventHandler : INotificationHandler<DomainEventNotification<BookingCreatedEvent>>
{
    private readonly ILogger<BookingCreatedEventHandler> _logger;
    private readonly ServiceBusSender _serviceBusSender;
    private readonly string _topicName = "booking";

    public BookingCreatedEventHandler(ILogger<BookingCreatedEventHandler> logger, ServiceBusClient serviceBusClient)
    {
        _logger = logger;
        _serviceBusSender = serviceBusClient.CreateSender(_topicName);
    }

    public async Task Handle(DomainEventNotification<BookingCreatedEvent> notification, CancellationToken ct)
    {
        var domainEvent = notification.DomainEvent;
        try
        {
            _logger.LogInformation("Iniciando envío de evento BookingCreated a Service Bus. Detalles del evento: {@Event}", domainEvent);

            var messageBody = JsonSerializer.Serialize(domainEvent);

            var message = new ServiceBusMessage(messageBody)
            {
                Subject = "BookingCreated",
            };

            message.ApplicationProperties.Add("eventType", "BookingCreated");
            message.ApplicationProperties.Add("timestamp", DateTime.UtcNow.ToString("o"));
            message.ApplicationProperties.Add("eventId", Guid.NewGuid().ToString());
            message.ApplicationProperties.Add("source", "BookingService");

            _logger.LogDebug("Mensaje de Service Bus preparado para envío: {@Message}", message);

            await _serviceBusSender.SendMessageAsync(message, ct);

            _logger.LogInformation("Evento BookingCreated enviado exitosamente a Service Bus. EventId: {EventId}", message.ApplicationProperties["eventId"]);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar evento BookingCreated a Service Bus. Topic: {TopicName}", _topicName);
            throw;
        }
    }
}
