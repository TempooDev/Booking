using Shared.Common;

namespace Booking.Booking.Application.TodoItems.Domain;

public sealed class TodoItemCompletedEvent(TodoItem item) : DomainEvent
{
    public TodoItem Item { get; } = item;
}