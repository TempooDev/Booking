using ErrorOr;
using FluentAssertions;
using NUnit.Framework;
using VerticalSliceArchitecture.Application.Domain.Todos;
using VerticalSliceArchitecture.Application.Features.TodoItems;
using VerticalSliceArchitecture.Application.Features.TodoLists;

using static VerticalSliceArchitecture.Application.IntegrationTests.Testing;

namespace VerticalSliceArchitecture.Application.IntegrationTests.TodoItems;
public class CreateTodoItemTests : TestBase
{
    [Test]
    public async Task ShouldRequireMinimumFields()
    {
        // Corregir la creación del comando para pasar los parámetros requeridos
        var command = new CreateTodoItemCommand(0, null);

        // Modificar la expectativa para adaptarse al nuevo sistema basado en ErrorOr
        var result = await SendAsync(command);

        // Verificar que el resultado contenga errores de validación
        result.IsError.Should().BeTrue();
        result.Errors.Should().NotBeEmpty();
        result.Errors.Should().Contain(e => e.Type == ErrorType.Validation);
    }

    [Test]
    public async Task ShouldCreateTodoItem()
    {
        var userId = await RunAsDefaultUserAsync();

        // Crear el comando con los parámetros requeridos
        var listId = await SendAsync(new CreateTodoListCommand("New List"));

        // Crear el comando con los parámetros requeridos
        var command = new CreateTodoItemCommand(
            ListId: listId.Value,
            Title: "Tasks");

        var result = await SendAsync(command);

        // Verificar que no hay errores y obtener el ID
        result.IsError.Should().BeFalse();
        var itemId = result.Value;

        var item = await FindAsync<TodoItem>(itemId);

        item.Should().NotBeNull();
        item!.ListId.Should().Be(command.ListId);
        item.Title.Should().Be(command.Title);
        item.CreatedBy.Should().Be(userId);
        item.Created.Should().BeCloseTo(DateTime.Now, TimeSpan.FromMilliseconds(10000));
        item.LastModifiedBy.Should().BeNull();
        item.LastModified.Should().BeNull();
    }
}
