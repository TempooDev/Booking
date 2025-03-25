using Booking.Application.Common.Interfaces;

namespace Booking.Application.Infrastructure.Services;

public class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}