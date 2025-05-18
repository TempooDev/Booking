using Booking.Core.Common;
using Booking.Core.Users.Application.Commands;
using Booking.Core.Users.Application.Queries;
using Booking.Core.Users.Domain.Entities;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Booking.Booking.Api.UseCases;

public static class UsersEndpoints
{
    public static IEndpointRouteBuilder MapUsersEndpoints(this IEndpointRouteBuilder app)
    {
        var usersGroup = app.MapGroup($"{ApiPaths.Root}/users").WithTags("Users");

        usersGroup.MapPost("/", async (ISender mediator, [FromBody] CreateUserCommand command) =>
        {
            var result = await mediator.Send(command);

            if (result.IsError)
            {
                return Results.BadRequest(result.Errors);
            }

            return Results.Created($"{ApiPaths.Root}/users/{result.Value}", result.Value);
        })
        .WithName("CreateUser")
        .WithDescription("Creates a new user")
        .WithSummary("Create a new user");

        usersGroup.MapGet("/", async (ISender mediator) =>
        {
            var result = await mediator.Send(new GetAllUsersQuery());

            if (result.IsError)
            {
                return Results.BadRequest(result.Errors);
            }

            return Results.Ok(result.Value);
        })
        .WithName("GetAllUsers")
        .WithDescription("Gets all users")
        .WithSummary("Get all users");

        usersGroup.MapGet("/role/{role}", async (ISender mediator, [FromRoute] UserRole role) =>
        {
            var result = await mediator.Send(new GetUsersByRoleQuery(role));

            if (result.IsError)
            {
                return Results.BadRequest(result.Errors);
            }

            return Results.Ok(result.Value);
        })
        .WithName("GetUsersByRole")
        .WithDescription("Gets users by role")
        .WithSummary("Get users by role");

        usersGroup.MapGet("/{id}", async (ISender mediator, [FromRoute] Guid id) =>
        {
            var result = await mediator.Send(new GetUserByIdQuery(id));

            if (result.IsError)
            {
                return Results.NotFound(result.Errors);
            }

            return Results.Ok(result.Value);
        })
        .WithName("GetUserById")
        .WithDescription("Gets a user by ID")
        .WithSummary("Get user by ID");

        usersGroup.MapDelete("/{id}", async (ISender mediator, [FromRoute] Guid id) =>
        {
            var result = await mediator.Send(new DeleteUserCommand(id));

            if (result.IsError)
            {
                return Results.BadRequest(result.Errors);
            }

            return Results.NoContent();
        })
        .WithName("DeleteUser")
        .WithDescription("Deletes a user by ID")
        .WithSummary("Delete user by ID");

        usersGroup.MapPut("/{id}", async (ISender mediator, [FromRoute] Guid id, [FromBody] UpdateUserCommand command) =>
        {
            if (id != command.Id)
            {
                return Results.BadRequest("ID in route does not match ID in body");
            }

            var result = await mediator.Send(command);

            if (result.IsError)
            {
                return Results.BadRequest(result.Errors);
            }

            return Results.Ok(result.Value);
        })
        .WithName("UpdateUser")
        .WithDescription("Updates a user by ID")
        .WithSummary("Update user by ID");

        return app;
    }
}
