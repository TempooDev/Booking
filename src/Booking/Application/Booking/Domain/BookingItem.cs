using Booking.Booking.Application.Booking.Domain.Events;
using Booking.Shared.Common.Exceptions;

using Shared.Common;

namespace Booking.Booking.Application.Booking.Domain
{
    public enum BookingStatus
    {
        Pending,
        Confirmed,
        Cancelled,
    }

    public class BookingItem : AuditableEntity, IHasDomainEvent
    {
        public Guid BookingId { get; set; }
        public Guid CustomerId { get; set; }
        public Guid SellerId { get; set; }
        public Guid ProductId { get; set; }
        public string? Location { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime? Reminder { get; set; }
        public bool Cancelled { get; set; }
        public bool Paid { get; set; }
        public BookingStatus Status { get; set; }
        public int NumberOfGuests { get; set; }
        public string? RoomType { get; set; }
        public decimal? AmountPaid { get; set; }

        private string? _notes;
        public string? Notes
        {
            get => _notes;
            set
            {
                if (value != null && value.Length > 1000)
                {
                    DomainEvents.Add(new BookingNoteChangedEvent(this));
                }

                _notes = value;
            }
        }

        public List<DomainEvent> DomainEvents { get; } = [];

        public BookingItem()
        {
            Status = BookingStatus.Pending;
        }

        public void ConfirmBooking()
        {
            if (Status == BookingStatus.Pending)
            {
                Status = BookingStatus.Confirmed;
                DomainEvents.Add(new BookingConfirmedEvent(this));
            }
        }

        public void CancelBooking()
        {
            if (Status != BookingStatus.Cancelled)
            {
                Status = BookingStatus.Cancelled;
                DomainEvents.Add(new BookingCancelledEvent(this));
            }
        }

        public int GetDurationInDays()
        {
            return (EndTime - StartTime).Days;
        }

        public Booking(Guid bookingId, Guid customerId, Guid sellerId, DateTime startTime, DateTime endTime, int numberOfGuests)
        {
            if (bookingId == Guid.Empty) {
                throw new DomainException($"{nameof(bookingId)} cannot be empty");
            }
            if (customerId == Guid.Empty) {
                throw new DomainException($"{nameof(customerId)} cannot be empty");
            }
            if (sellerId == Guid.Empty) {
                throw new DomainException($"{nameof(sellerId)} cannot be empty");
            }
            if (startTime >= endTime)
            {
                throw new DomainException($"{nameof(startTime)} cannot be empty");
            }

            if (numberOfGuests <= 0)
            {
                throw new DomainException($"{nameof(numberOfGuests)} cannot be empty");
            }

            BookingId = bookingId;
            CustomerId = customerId;
            SellerId = sellerId;
            StartTime = startTime;
            EndTime = endTime;
            NumberOfGuests = numberOfGuests;
            Status = BookingStatus.Pending;
        }
    }

    public record BookingRecord(Guid BookingId, Guid CustomerId, Guid SellerId, DateTime StartTime, DateTime EndTime, int NumberOfGuests);
}
