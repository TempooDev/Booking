using Shared.Common;

namespace Booking.Booking.Application.Booking.Domain.Events
{
    public sealed class BookingCreatedEvent(BookingItem booking) : DomainEvent
    {
        public BookingItem Booking { get; } = booking;
    }
}
