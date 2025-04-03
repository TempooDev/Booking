using System.Text.Json;

using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;

using Booking.Booking.Application.Booking.Domain.Events;

using MediatR;

using Microsoft.Extensions.Logging;

using Shared.Common.Models;

namespace Booking.Booking.Application.Booking.Application.EventHandlers;

internal sealed class BookingCreatedEventHandler(ILogger<BookingCreatedEventHandler> logger, EventHubProducerClient eventHubClient) : INotificationHandler<DomainEventNotification<BookingCreatedEvent>>
{
    private readonly ILogger<BookingCreatedEventHandler> _logger = logger;

    private readonly EventHubProducerClient _eventHubClient = eventHubClient;
    public async Task Handle(DomainEventNotification<BookingCreatedEvent> notification, CancellationToken ct)
    {
        var domainEvent = notification.DomainEvent;
        try
        {
            _logger.LogInformation("Iniciando envío de evento BookingCreated. Detalles del evento: {@Event}", domainEvent);

            // Primero verificamos la conexión del cliente
            _logger.LogDebug("EventHub Client configurado para: {FullyQualifiedNamespace}", _eventHubClient.FullyQualifiedNamespace);

            // Creamos el evento con más metadatos para mejor trazabilidad
            var eventData = new EventData(JsonSerializer.SerializeToUtf8Bytes(domainEvent));
            eventData.Properties.Add("eventType", "BookingCreated");
            eventData.Properties.Add("timestamp", DateTime.UtcNow.ToString("o"));
            eventData.Properties.Add("eventId", Guid.NewGuid().ToString());
            eventData.Properties.Add("source", "BookingService");

            _logger.LogDebug("Evento preparado para envío: {@EventProperties}", eventData.Properties);

            // Enviamos directamente sin batch para simplificar y asegurar el envío
            await _eventHubClient.SendAsync(new[] { eventData }, ct);

            _logger.LogInformation(
                "Evento BookingCreated enviado exitosamente. EventId: {EventId}",
                eventData.Properties["eventId"]);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error al enviar evento BookingCreated. Namespace: {Namespace}, EventHub: {EventHub}",
                _eventHubClient.FullyQualifiedNamespace,
                _eventHubClient.EventHubName);
            throw;
        }
    }
}
