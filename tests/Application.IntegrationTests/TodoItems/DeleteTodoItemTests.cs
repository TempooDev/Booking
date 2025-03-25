using FluentAssertions;

using NUnit.Framework;

using VerticalSliceArchitecture.Application.Domain.Todos;
using VerticalSliceArchitecture.Application.Features.TodoItems;
using VerticalSliceArchitecture.Application.Features.TodoLists;

using static VerticalSliceArchitecture.Application.IntegrationTests.Testing;

namespace VerticalSliceArchitecture.Application.IntegrationTests.TodoItems;
public class DeleteTodoItemTests : TestBase
{
    [Test]
    public async Task ShouldRequireValidTodoItemId()
    {
        var command = new DeleteTodoItemCommand(99);

        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<Exception>();
    }

    [Test]
    public async Task ShouldDeleteTodoItem()
    {
        var listId = await SendAsync(new CreateTodoListCommand("New List"));

        var itemId = await SendAsync(new CreateTodoItemCommand(listId.Value, "New Item"));

        await SendAsync(new DeleteTodoItemCommand(itemId.Value));

        var item = await FindAsync<TodoItem>(itemId);

        item.Should().BeNull();
    }
}
