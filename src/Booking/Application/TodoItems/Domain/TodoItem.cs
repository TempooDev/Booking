using Booking.Booking.Application.TodoLists.Domain;

using Shared.Common;

namespace Booking.Booking.Application.TodoItems.Domain;

public class TodoItem : AuditableEntity, IHasDomainEvent
{
    public int ListId { get; set; }

    public string? Title { get; set; }

    public string? Note { get; set; }

    public PriorityLevel Priority { get; set; }

    public DateTime? Reminder { get; set; }

    private bool _done;
    public bool Done
    {
        get => _done;
        set
        {
            if (value && _done == false)
            {
                DomainEvents.Add(new TodoItemCompletedEvent(this));
            }

            _done = value;
        }
    }

    public TodoList List { get; set; } = null!;

    public List<DomainEvent> DomainEvents { get; } = [];
}

public enum PriorityLevel
{
    None = 0,
    Low = 1,
    Medium = 2,
    High = 3,
}

public record TodoItemRecord(string? Title, bool Done);