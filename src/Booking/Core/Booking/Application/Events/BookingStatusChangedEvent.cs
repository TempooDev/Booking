using Booking.Core.Booking.Domain;

using MediatR;

namespace Booking.Core.Booking.Application.Events;

/// <summary>
/// Evento que se dispara cuando cambia el estado de una reserva.
/// </summary>
public record BookingStatusChangedEvent(
    Guid BookingId,
    BookingStatus OldStatus,
    BookingStatus NewStatus,
    Guid CustomerId,
    Guid SellerId) : INotification;
