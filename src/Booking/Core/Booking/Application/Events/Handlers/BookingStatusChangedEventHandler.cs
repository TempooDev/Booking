using MediatR;

using Microsoft.Extensions.Logging;

namespace Booking.Core.Booking.Application.Events.Handlers;

/// <summary>
/// Manejador para el evento de cambio de estado de una reserva.
/// </summary>
public class BookingStatusChangedEventHandler : INotificationHandler<BookingStatusChangedEvent>
{
    private readonly ILogger<BookingStatusChangedEventHandler> _logger;

    public BookingStatusChangedEventHandler(ILogger<BookingStatusChangedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(BookingStatusChangedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Booking {BookingId} status changed from {OldStatus} to {NewStatus}",
            notification.BookingId,
            notification.OldStatus,
            notification.NewStatus);

        return Task.CompletedTask;
    }
}
