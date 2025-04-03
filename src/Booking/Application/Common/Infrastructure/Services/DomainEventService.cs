using System.Text.Json;

using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;

using MediatR;

using Microsoft.Extensions.Logging;

using Shared.Common;
using Shared.Common.Interfaces;
using Shared.Common.Models;

namespace Booking.Booking.Application.Common.Infrastructure.Services;

public class DomainEventService(ILogger<DomainEventService> logger, IPublisher mediator, EventHubProducerClient eventHubClient) : IDomainEventService
{
    private readonly ILogger<DomainEventService> _logger = logger;
    private readonly IPublisher _mediator = mediator;
    private readonly EventHubProducerClient _eventHubClient = eventHubClient;
    public async Task Publish(DomainEvent domainEvent)
    {
        _logger.LogInformation("Publishing domain event. Event - {event}", domainEvent.GetType().Name);
        await _mediator.Publish(GetNotificationCorrespondingToDomainEvent(domainEvent));
    }

    private static INotification GetNotificationCorrespondingToDomainEvent(DomainEvent domainEvent)
    {
        return (INotification)Activator.CreateInstance(
            typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType()), domainEvent)!;
    }
}