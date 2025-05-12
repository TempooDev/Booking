using Booking.Booking.Application.Booking.Application.Commands;
using Booking.Booking.Application.Booking.Application.Queries;
using Booking.Booking.Application.Common;
using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using Shared.Common.Models;

namespace Booking.Api;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapBookingEndpoints(this IEndpointRouteBuilder app)
    {
        var bookingGroup = app.MapGroup($"{ApiPaths.Root}/booking")
            .WithTags(ApiPaths.Booking);

        // CreateBooking endpoint
        bookingGroup.MapPost("/", async (ISender mediator, [FromBody] CreateBookingCommand command, CancellationToken cancellationToken) =>
        {
            var result = await mediator.Send(command, cancellationToken);
            return result.Match(
                id => Results.Ok(id),
                errors => Results.Problem(string.Join("; ", errors.Select(e => e.Description))));
        })
        .WithName("CreateBooking")
        .WithDescription("Creates a new booking")
        .WithSummary("Create a new booking");

        // GetBookingWithPagination endpoint - Fix parameter binding
        bookingGroup.MapGet("/", async (
            ISender mediator,
            [FromQuery] Guid? bookingId,
            [FromQuery] Guid? customerId,
            [FromQuery] Guid? sellerId,
            [FromQuery] Guid? productId,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            CancellationToken cancellationToken = default) =>
        {
            var query = new GetBookingWithPaginationQuery(
                bookingId, customerId, sellerId, productId, pageNumber, pageSize);

            var result = await mediator.Send(query, cancellationToken);
            return result.Match(
                data => Results.Ok(data),
                errors => Results.Problem(string.Join("; ", errors.Select(e => e.Description))));
        })
        .WithName("GetBookingWithPagination")
        .WithDescription("Gets a paginated list of bookings with optional filters")
        .WithSummary("Get bookings with pagination and filtering");

        return app;
    }
}
