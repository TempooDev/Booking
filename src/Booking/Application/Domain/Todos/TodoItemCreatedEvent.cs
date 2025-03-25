using Booking.Application.Common;

namespace Booking.Application.Domain.Todos;

internal sealed class TodoItemCreatedEvent(TodoItem item) : DomainEvent
{
    public TodoItem Item { get; } = item;
}