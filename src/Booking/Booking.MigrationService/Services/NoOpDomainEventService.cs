using Shared.Common;
using Shared.Common.Interfaces;
using Shared.Common.Models;

namespace Booking.Booking.MigrationService.Services;

public class NoOpDomainEventService : IDomainEventService
{
    public Task Publish(DomainEvent domainEvent)
    {
        return Task.CompletedTask;
    }
}