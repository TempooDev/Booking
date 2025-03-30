using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Shared.Common;

namespace Booking.Booking.Application.Booking.Domain.Events
{
    public sealed class BookingCancelledEvent(BookingItem booking) : DomainEvent
    {
        public BookingItem Booking { get; } = booking;
    }
}
