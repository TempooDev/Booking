using Shared.Common;

namespace Booking.Booking.Application.Domain.Todos;

internal sealed class TodoItemCompletedEvent(TodoItem item) : DomainEvent
{
    public TodoItem Item { get; } = item;
}