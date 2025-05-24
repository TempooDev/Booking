using Azure.Messaging.ServiceBus;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Booking.Core.Booking.Application.Events.Handlers;

/// <summary>
/// Manejador para el evento de cambio de estado de una reserva.
/// </summary>
public class BookingStatusChangedEventHandler : INotificationHandler<BookingStatusChangedEvent>
{
    private readonly ILogger<BookingStatusChangedEventHandler> _logger;
    private readonly ServiceBusSender _serviceBusSender;
    private readonly string _topicName = "booking";

    public BookingStatusChangedEventHandler(ILogger<BookingStatusChangedEventHandler> logger, ServiceBusClient serviceBusClient)
    {
        _logger = logger;
        _serviceBusSender = serviceBusClient.CreateSender(_topicName);
    }

    public async Task Handle(BookingStatusChangedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Booking {BookingId} status changed from {OldStatus} to {NewStatus}",
            notification.BookingId,
            notification.OldStatus,
            notification.NewStatus);

        // Serializar el evento y enviarlo a Service Bus siguiendo el patrón de BookingCreatedEventHandler
        var messageBody = System.Text.Json.JsonSerializer.Serialize(notification);

        var message = new Azure.Messaging.ServiceBus.ServiceBusMessage(messageBody)
        {
            Subject = "BookingStatusChanged",
            ContentType = "application/json",
        };

        message.ApplicationProperties.Add("eventType", "BookingStatusChanged");
        message.ApplicationProperties.Add("timestamp", DateTime.UtcNow.ToString("o"));
        message.ApplicationProperties.Add("eventId", Guid.NewGuid().ToString());
        message.ApplicationProperties.Add("source", "BookingService");

        await _serviceBusSender.SendMessageAsync(message, cancellationToken);

        try
        {
            await _serviceBusSender.SendMessageAsync(message, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al enviar el evento BookingStatusChanged a Service Bus. BookingId: {BookingId}", notification.BookingId);
            throw;
        }

        _logger.LogInformation("Evento BookingStatusChanged enviado exitosamente a Service Bus. EventId: {EventId}", message.ApplicationProperties["eventId"]);
    }
}
