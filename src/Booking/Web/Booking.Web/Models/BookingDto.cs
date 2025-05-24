using System;

namespace Booking.Web.Models;

public enum BookingStatus
{
    Pending,
    Confirmed,
    Cancelled,
}

public class BookingDto
{
    public Guid Id { get; set; }
    public Guid CustomerId { get; set; }
    public Guid SellerId { get; set; }
    public Guid ProductId { get; set; }
    public string? Location { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public DateTime? Reminder { get; set; }
    public BookingStatus Status { get; set; }
    public int NumberOfGuests { get; set; }
    public string? RoomType { get; set; }
    public string? Notes { get; set; }
    public bool Paid { get; set; }
    public decimal? AmountPaid { get; set; }
    public DateTime Created { get; set; }
    public DateTime? LastModified { get; set; }
}
