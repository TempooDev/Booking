using Booking.Booking.Application.Booking.Domain.Events;

using MediatR;

using Microsoft.Extensions.Logging;

using Shared.Common.Models;

namespace Booking.Booking.Application.Booking.Application.EventHandlers;

internal sealed class BookingCreatedEventHandler(ILogger<BookingCreatedEventHandler> logger) : INotificationHandler<DomainEventNotification<BookingCreatedEvent>>
{
    private readonly ILogger<BookingCreatedEventHandler> _logger = logger;
    public Task Handle(DomainEventNotification<BookingCreatedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;
        _logger.LogInformation("Booking {DomainEvent}", domainEvent.GetType().Name);
        return Task.CompletedTask;
    }
}
