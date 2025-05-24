using Booking.Core.Common.Infrastructure.Persistence;

using ErrorOr;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Booking.Core.Booking.Application.Queries;

public record GetBookingByIdQuery(Guid BookingId) : IRequest<ErrorOr<BookingBriefResponse>>;

internal sealed class GetBookingByIdQueryHandler(ApplicationDbContext context) : IRequestHandler<GetBookingByIdQuery, ErrorOr<BookingBriefResponse>>
{
    private readonly ApplicationDbContext _context = context;

    public async Task<ErrorOr<BookingBriefResponse>> Handle(GetBookingByIdQuery request, CancellationToken cancellationToken)
    {
        var booking = await _context.Bookings
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == request.BookingId, cancellationToken);

        if (booking is null)
        {
            return Error.NotFound(description: "Booking not found.");
        }

        return new BookingBriefResponse(
            booking.Id,
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