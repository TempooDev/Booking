using Booking.Booking.Application.Booking.Domain.Events;
using Booking.Shared.Common.Exceptions;

using Shared.Common;

namespace Booking.Booking.Application.Booking.Domain;

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

    public List<DomainEvent> DomainEvents { get; } = [];
}

public record BookingRecord(Guid BookingId, Guid CustomerId, Guid SellerId, DateTime StartTime, DateTime EndTime, int NumberOfGuests);
