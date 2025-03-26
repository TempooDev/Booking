using System.Globalization;

using Booking.Booking.Application.TodoItems.Domain;

using CsvHelper.Configuration;

namespace Booking.Booking.Application.TodoItems.Infrastructure.Files;

public class TodoItemRecordMap : ClassMap<TodoItemRecord>
{
    public TodoItemRecordMap()
    {
        AutoMap(CultureInfo.InvariantCulture);

        Map(m => m.Done).ConvertUsing(c => c.Done ? "Yes" : "No");
    }
}