using System;

namespace Booking.Web.Models;

public class UpdateBookingDto
{
    public Guid BookingId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid SellerId { get; set; }
    public Guid ProductId { get; set; }
    public string? Location { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int NumberOfGuests { get; set; }
    public string? RoomType { get; set; }
    public string? Notes { get; set; }
}
