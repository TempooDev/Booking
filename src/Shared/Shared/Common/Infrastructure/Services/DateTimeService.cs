using Shared.Common.Interfaces;

namespace Shared.Common.Infrastructure.Services;

public class DateTimeService : IDateTime
{
    public DateTime Now => DateTime.UtcNow;
}