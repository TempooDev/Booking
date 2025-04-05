using System.Text.Json;

using Azure.Messaging.ServiceBus;

using MediatR;

using Microsoft.Extensions.Logging;

using Shared.Common;
using Shared.Common.Interfaces;
using Shared.Common.Models;

namespace Booking.Booking.Application.Common.Infrastructure.Services;

public class DomainEventService : IDomainEventService
{
    private readonly ILogger<DomainEventService> _logger;
    private readonly IPublisher _mediator;
    private readonly ServiceBusClient _serviceBusClient;
    private readonly string _topicName = "booking";

    public DomainEventService(ILogger<DomainEventService> logger, IPublisher mediator, ServiceBusClient serviceBusClient)
    {
        _logger = logger;
        _mediator = mediator;
        _serviceBusClient = serviceBusClient;
    }

    public async Task Publish(DomainEvent domainEvent)
    {
        _logger.LogInformation("Publishing domain event. Event - {event}", domainEvent.GetType().Name);
        await _mediator.Publish(GetNotificationCorrespondingToDomainEvent(domainEvent));
        await PublishToServiceBus(domainEvent);
    }

    private static INotification GetNotificationCorrespondingToDomainEvent(DomainEvent domainEvent)
    {
        return (INotification)Activator.CreateInstance(
            typeof(DomainEventNotification<>).MakeGenericType(domainEvent.GetType()), domainEvent)!;
    }

    private async Task PublishToServiceBus(DomainEvent domainEvent)
    {
        try
        {
            // Serialize the domain event to JSON
            string messageBody = JsonSerializer.Serialize(domainEvent);

            // Create a message
            ServiceBusMessage message = new ServiceBusMessage(messageBody);

            // Use a using statement to ensure the sender is properly disposed of after use
            await using (ServiceBusSender sender = _serviceBusClient.CreateSender(_topicName))
            {
                // Send the message to the topic
                await sender.SendMessageAsync(message);
                _logger.LogInformation("Sent message to topic: {TopicName}", _topicName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while sending message to topic: {TopicName}", _topicName);
        }
    }
}