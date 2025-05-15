using Shared.Common;

namespace Booking.Core.Booking.Domain.Events
{
    public sealed class BookingConfirmedEvent(BookingItem booking) : DomainEvent
    {
        public BookingItem Booking { get; } = booking;
    }
}
