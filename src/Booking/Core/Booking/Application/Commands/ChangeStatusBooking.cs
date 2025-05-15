using Booking.Common.Errors;
using Booking.Core.Booking.Application.Events;
using Booking.Core.Booking.Domain;
using Booking.Core.Common.Infrastructure.Persistence;

using ErrorOr;

using MediatR;

namespace Booking.Core.Booking.Application.Commands;

public record ChangeStatusBookingCommand(
    Guid BookingId,
    BookingStatus Status) : IRequest<ErrorOr<bool>>;

internal sealed class ChangeStatusBookingCommandHandler : IRequestHandler<ChangeStatusBookingCommand, ErrorOr<bool>>
{
    private readonly ApplicationDbContext _context;
    private readonly IPublisher _publisher; // Añadir IPublisher para eventos

    public ChangeStatusBookingCommandHandler(
        ApplicationDbContext context,
        IPublisher publisher)
    {
        _context = context;
        _publisher = publisher;
    }

    public async Task<ErrorOr<bool>> Handle(ChangeStatusBookingCommand request, CancellationToken cancellationToken)
    {
        var booking = await _context.Bookings.FindAsync(new object[] { request.BookingId }, cancellationToken);

        if (booking is null)
        {
            return Errors.Booking.NotFound;
        }

        var oldStatus = booking.Status;
        booking.Status = request.Status;

        await _context.SaveChangesAsync(cancellationToken);

        // Publicar evento de cambio de estado
        await _publisher.Publish(
            new BookingStatusChangedEvent(
            booking.Id,
            oldStatus,
            booking.Status,
            booking.CustomerId,
            booking.SellerId),
            cancellationToken);

        return true;
    }
}