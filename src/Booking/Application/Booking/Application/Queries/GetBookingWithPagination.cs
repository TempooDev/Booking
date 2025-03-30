using Booking.Booking.Application.Booking.Domain;
using Booking.Booking.Application.Common.Infrastructure.Persistence;
using Booking.Shared.Common;

using ErrorOr;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using Shared.Common.Mappings;
using Shared.Common.Models;

namespace Booking.Booking.Application.Booking.Application.Queries;
public class GetBookingWithPaginationController : ApiControllerBase
{
    [HttpGet("/api/booking")]
    public async Task<IActionResult> GetBookingWithPagination([FromQuery] GetBookingWithPaginationQuery query)
    {
        var result = await Mediator.Send(query);
        return result.Match(
            Ok,
            Problem);
    }
}

public record BookingBriefResponse(Guid Id, Guid CustomerId, Guid SellerId, Guid ProductId, string? Location, DateTime StartTime, DateTime EndTime, int NumberOfGuests, string? RoomType, string? Notes);
public record GetBookingWithPaginationQuery(Guid? CustomerId, Guid? SellerId, Guid? ProductId, int PageNumber = 1, int PageSize = 10) : IRequest<ErrorOr<PaginatedList<BookingBriefResponse>>>;

internal sealed class GetBookingWithPaginationQueryValidator : AbstractValidator<GetBookingWithPaginationQuery>
{
    public GetBookingWithPaginationQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1).WithMessage("PageNumber at least greater than or equal to 1.");
        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1).WithMessage("PageSize at least greater than or equal to 1.");
        RuleFor(x => x.CustomerId)
            .NotEmpty().WithMessage("CustomerId is required.");
        RuleFor(x => x.SellerId)
            .NotEmpty().WithMessage("SellerId is required.");
        RuleFor(x => x.ProductId)
            .NotEmpty().WithMessage("ProductId is required.");
    }
}

internal sealed class GetBookingWithPaginationQueryHandler(ApplicationDbContext context) : IRequestHandler<GetBookingWithPaginationQuery, ErrorOr<PaginatedList<BookingBriefResponse>>>
{
    private readonly ApplicationDbContext _context = context;
    public async Task<ErrorOr<PaginatedList<BookingBriefResponse>>> Handle(GetBookingWithPaginationQuery request, CancellationToken cancellationToken)
    {
        var paginatedList = await _context.Bookings
            .OrderBy(booking => booking.StartTime)
            .Select(booking => ToDto(booking))
            .PaginatedListAsync(request.PageNumber, request.PageSize);
        return paginatedList;
    }

    private static BookingBriefResponse ToDto(BookingItem booking)
    {
        return new BookingBriefResponse(
            booking.BookingId,
            booking.CustomerId,
            booking.SellerId,
            booking.ProductId,
            booking.Location,
            booking.StartTime,
            booking.EndTime,
            booking.NumberOfGuests,
            booking.RoomType,
            booking.Notes);
    }
}