using Shared.Common.Interfaces;

namespace Booking.Booking.Application.Infrastructure.Services;

public class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.Now;
}