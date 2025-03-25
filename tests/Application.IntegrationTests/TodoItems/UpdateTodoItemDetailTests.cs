using FluentAssertions;

using NUnit.Framework;

using VerticalSliceArchitecture.Application.Domain.Todos;
using VerticalSliceArchitecture.Application.Features.TodoItems;
using VerticalSliceArchitecture.Application.Features.TodoLists;

using static VerticalSliceArchitecture.Application.IntegrationTests.Testing;

namespace VerticalSliceArchitecture.Application.IntegrationTests.TodoItems;
public class UpdateTodoItemDetailTests : TestBase
{
    [Test]
    public async Task ShouldRequireValidTodoItemId()
    {
        var command = new UpdateTodoItemCommand(99, "New Title", false); // Fixed constructor call
        await FluentActions.Invoking(() => SendAsync(command)).Should().ThrowAsync<Exception>();
    }

    [Test]
    public async Task ShouldUpdateTodoItem()
    {
        var userId = await RunAsDefaultUserAsync();

        var listId = await SendAsync(new CreateTodoListCommand("New List"));

        var itemId = await SendAsync(new CreateTodoItemCommand(listId.Value, "New Item"));

        var command = new UpdateTodoItemDetailCommand(itemId.Value, listId.Value, PriorityLevel.High, "This is the note.");

        await SendAsync(command);

        var item = await FindAsync<TodoItem>(itemId);

        item.Should().NotBeNull();
        item!.ListId.Should().Be(command.ListId);
        item.Note.Should().Be(command.Note);
        item.Priority.Should().Be(command.Priority);
        item.LastModifiedBy.Should().NotBeNull();
        item.LastModifiedBy.Should().Be(userId);
        item.LastModified.Should().NotBeNull();
        item.LastModified.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(10000));
    }
}
