using Booking.Booking.Application.TodoItems.Domain;
using Booking.Booking.Application.TodoItems.Domain.ValueObjects;

using Shared.Common;

namespace Booking.Booking.Application.TodoLists.Domain;

public class TodoList : AuditableEntity
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public Colour Colour { get; set; } = Colour.White;

    public IList<TodoItem> Items { get; private set; } = new List<TodoItem>();
}