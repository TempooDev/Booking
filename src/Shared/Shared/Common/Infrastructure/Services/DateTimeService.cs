using Shared.Common.Interfaces;

namespace Booking.Shared.Application.Common.Infrastructure.Services;

public class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.UtcNow;
}