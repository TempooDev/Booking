using Booking.Booking.Application.Domain.Todos;

using Shared.Common;

namespace Booking.Application.Domain.Todos;

internal sealed class TodoItemCreatedEvent(TodoItem item) : DomainEvent
{
    public TodoItem Item { get; } = item;
}