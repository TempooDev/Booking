using Booking.Booking.Application.TodoItems.Application.EventHandlers;
using Booking.Booking.Application.TodoItems.Domain;

using MediatR;

using Microsoft.Extensions.Logging;

using Moq;

using Shared.Common.Models;

namespace Booking.Application.UnitTests.TodoItems.EventHandler
{
    public class TodoItemCompletedEventHandlerTests
    {
        private readonly Mock<ILogger<TodoItemCompletedEventHandler>> _loggerMock;
        private readonly TodoItemCompletedEventHandler _handler;

        public TodoItemCompletedEventHandlerTests()
        {
            _loggerMock = new Mock<ILogger<TodoItemCompletedEventHandler>>();
            _handler = new TodoItemCompletedEventHandler(_loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ShouldLogInformation()
        {
            // Arrange
            var todoItem = new TodoItem { Id = 1, Title = "Test Item" };
            var domainEvent = new TodoItemCompletedEvent(todoItem);
            var notification = new DomainEventNotification<TodoItemCompletedEvent>(domainEvent);
            var cancellationToken = CancellationToken.None;

            // Act
            await _handler.Handle(notification, cancellationToken);

            // Assert
            Assert.True(_loggerMock.Invocations.Count >= 1);
        }
    }
}