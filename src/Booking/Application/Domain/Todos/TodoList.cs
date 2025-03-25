using Booking.Booking.Application.Domain.Todos;
using Booking.Booking.Application.Domain.ValueObjects;

using Shared.Common;

namespace Booking.Application.Domain.Todos;

public class TodoList : AuditableEntity
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public Colour Colour { get; set; } = Colour.White;

    public IList<TodoItem> Items { get; private set; } = new List<TodoItem>();
}