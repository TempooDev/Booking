using Booking.Booking.Application.TodoItems.Domain;

namespace Booking.Application.Common.Interfaces;

public interface ICsvFileBuilder
{
    byte[] BuildTodoItemsFile(IEnumerable<TodoItemRecord> records);
}