using Auth.Core.Auth.RegisterUser.Application;
using Auth.Core.Common;


namespace Booking.Auth.Api.UseCases;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var authGroup = app.MapGroup($"{ApiPaths.Root}/auth").WithTags(ApiPaths.Auth);

        authGroup.MapPost("/register", async (ISender sender, [FromBody] RegisterUserCommand command, CancellationToken ct) =>
        {
            var result = await mediator.Send(command, ct);
            return result.Match(
                value => Results.Ok(value),
                errors => Results.Problem(string.Join("; ", errors.Select(e => e.Description)))
            );
        })
            .WithName("RegisterUser")
            .WithDescription("Register a user on System")
            .WithSummary("");

    }
}