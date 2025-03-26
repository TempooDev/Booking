using Shared.Common;

namespace Booking.Booking.Application.TodoItems.Domain;

internal sealed class TodoItemDeletedEvent(TodoItem item) : DomainEvent
{
    public TodoItem Item { get; } = item;
}