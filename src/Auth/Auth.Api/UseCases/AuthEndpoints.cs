using Auth.Core.Application.Commands;
using Auth.Core.Application.Queries;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Booking.Auth.Api.UseCases;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        app.MapPost("/auth/register", async (RegisterUserCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.BadRequest(result.Errors);
        });

        app.MapPost("/auth/login", async (LoginUserCommand command, IMediator mediator) =>
        {
            var result = await mediator.Send(command);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.Unauthorized(result.Errors);
        });
    }
}