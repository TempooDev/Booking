using Booking.Application.Common;
using Booking.Application.Common.Interfaces;
using Booking.Application.Common.Models;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Booking.Application.Infrastructure.Services;

public class DomainEventService : IDomainEventService
{
    private readonly ILogger<DomainEventService> _logger;
    private readonly IPublisher _mediator;

    public DomainEventService(ILogger<DomainEventService> logger, IPublisher mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public Task Publish(DomainEvent domainEvent)
    {
        _logger.LogInformation("Publishing domain event. Event - {event}", domainEvent.GetType().Name);
        return _mediator.Publish(GetNotificationCorrespondingToDomainEvent(domainEvent));
    }

    private static INotification GetNotificationCorrespondingToDomainEvent(DomainEvent domainEvent)
    {
        return (INotification)Activator.CreateInstance(
            typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType()), domainEvent)!;
    }
}