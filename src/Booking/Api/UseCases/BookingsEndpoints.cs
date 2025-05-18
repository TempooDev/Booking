using Booking.Core.Booking.Application.Commands;
using Booking.Core.Booking.Application.Queries;
using Booking.Core.Booking.Domain;
using Booking.Core.Common;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Booking.Booking.Api.UseCases;

public static class BookingsEndpoints
{
    public static IEndpointRouteBuilder MapBookingEndpoints(this IEndpointRouteBuilder app)
    {
        var bookingGroup = app.MapGroup($"{ApiPaths.Root}/booking")
            .WithTags(ApiPaths.Booking);

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

        bookingGroup.MapPut("/{bookingId}", async (
            ISender mediator,
            [FromRoute] Guid bookingId,
            [FromBody] UpdateBookingCommand command,
            CancellationToken cancellationToken) =>
        {
            if (bookingId != command.BookingId)
            {
                return Results.BadRequest("Route bookingId and command BookingId must match");
            }

            var result = await mediator.Send(command, cancellationToken);
            return result.Match(
                id => Results.Ok(id),
                errors => Results.Problem(string.Join("; ", errors.Select(e => e.Description))));
        })
        .WithName("UpdateBooking")
        .WithDescription("Updates an existing booking")
        .WithSummary("Update booking details");

        bookingGroup.MapGet("/{bookingId}", async (
            ISender mediator,
            [FromRoute] Guid bookingId,
            CancellationToken cancellationToken) =>
        {
            var query = new GetBookingByIdQuery(bookingId);
            var result = await mediator.Send(query, cancellationToken);
            return result.Match(
                booking => Results.Ok(booking),
                errors => Results.Problem(string.Join("; ", errors.Select(e => e.Description))));
        })
        .WithName("GetBookingById")
        .WithDescription("Gets booking details by ID")
        .WithSummary("Get a specific booking");

        bookingGroup.MapPatch("/{bookingId}/status", async (
            ISender mediator,
            [FromRoute] Guid bookingId,
            [FromBody] BookingStatus status,
            CancellationToken cancellationToken) =>
        {
            var command = new ChangeStatusBookingCommand(bookingId, status);
            var result = await mediator.Send(command, cancellationToken);
            return result.Match(
                success => Results.Ok(success),
                errors => Results.Problem(string.Join("; ", errors.Select(e => e.Description))));
        })
        .WithName("ChangeBookingStatus")
        .WithDescription("Changes the status of a booking")
        .WithSummary("Update booking status");

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
