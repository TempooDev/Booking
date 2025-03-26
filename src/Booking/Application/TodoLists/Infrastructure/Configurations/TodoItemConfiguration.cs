using Booking.Booking.Application.TodoItems.Domain;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Booking.Booking.Application.TodoLists.Infrastructure.Configurations;

public class TodoItemConfiguration : IEntityTypeConfiguration<TodoItem>
{
    public void Configure(EntityTypeBuilder<TodoItem> builder)
    {
        builder.Ignore(e => e.DomainEvents);

        builder.Property(t => t.Title)
            .HasMaxLength(200)
            .IsRequired();
    }
}