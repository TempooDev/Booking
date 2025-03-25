using Booking.Application.Common;

namespace Booking.Application.Domain.Todos;

internal sealed class TodoItemDeletedEvent(TodoItem item) : DomainEvent
{
    public TodoItem Item { get; } = item;
}