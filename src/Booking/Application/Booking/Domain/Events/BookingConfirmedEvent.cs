using Shared.Common;

namespace Booking.Booking.Application.Booking.Domain.Events
{
    public sealed class BookingConfirmedEvent(BookingItem booking) : DomainEvent
    {
        public BookingItem Booking { get; } = booking;
    }
}
