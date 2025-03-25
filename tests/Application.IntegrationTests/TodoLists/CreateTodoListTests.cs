using System.ComponentModel.DataAnnotations;

using FluentAssertions;

using NUnit.Framework;

using VerticalSliceArchitecture.Application.Domain.Todos;
using VerticalSliceArchitecture.Application.Features.TodoLists;

using static VerticalSliceArchitecture.Application.IntegrationTests.Testing;

namespace VerticalSliceArchitecture.Application.IntegrationTests.TodoLists;
public class CreateTodoListTests : TestBase
{
    [Test]
    public async Task ShouldRequireMinimumFields()
    {
        var command = new CreateTodoListCommand(Title: null);
        await FluentActions.Invoking(() => SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task ShouldRequireUniqueTitle()
    {
        await SendAsync(new CreateTodoListCommand("Shopping"));

        var command = new CreateTodoListCommand("Shopping");

        await FluentActions.Invoking(() =>
            SendAsync(command)).Should().ThrowAsync<ValidationException>();
    }

    [Test]
    public async Task ShouldCreateTodoList()
    {
        var userId = await RunAsDefaultUserAsync();

        var command = new CreateTodoListCommand("Tasks");

        var id = await SendAsync(command);

        var list = await FindAsync<TodoList>(id);

        list.Should().NotBeNull();
        list!.Title.Should().Be(command.Title);
        list.CreatedBy.Should().Be(userId);
        list.Created.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(10000));
    }
}
