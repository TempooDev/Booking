using Booking.Common.Errors;
using Booking.Core.Common.Infrastructure.Persistence;

using ErrorOr;

using FluentValidation;

using MediatR;

namespace Booking.Core.Booking.Application.Commands;

public record UpdateBookingCommand(
    Guid BookingId,
    Guid CustomerId,
    Guid SellerId,
    Guid ProductId,
    string? Location,
    DateTime StartTime,
    DateTime EndTime,
    int NumberOfGuests,
    string? RoomType,
    string? Notes) : IRequest<ErrorOr<Guid>>;

internal sealed class UpdateBookingCommandValidator : AbstractValidator<UpdateBookingCommand>
{
    public UpdateBookingCommandValidator()
    {
        RuleFor(v => v.BookingId)
            .NotEmpty();
        RuleFor(v => v.CustomerId)
            .NotEmpty();
        RuleFor(v => v.SellerId)
            .NotEmpty();
        RuleFor(v => v.ProductId)
            .NotEmpty();
        RuleFor(v => v.Location)
            .MaximumLength(200);
        RuleFor(v => v.StartTime)
            .NotEmpty()
            .LessThan(v => v.EndTime);
        RuleFor(v => v.EndTime)
            .NotEmpty()
            .GreaterThan(v => v.StartTime);
        RuleFor(v => v.NumberOfGuests)
            .NotEmpty();
        RuleFor(v => v.RoomType)
            .MaximumLength(200);
        RuleFor(v => v.Notes)
            .MaximumLength(1000);
    }
}

internal sealed class UpdateBookingCommandHandler : IRequestHandler<UpdateBookingCommand, ErrorOr<Guid>>
{
    private readonly ApplicationDbContext _context;

    public UpdateBookingCommandHandler(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ErrorOr<Guid>> Handle(UpdateBookingCommand request, CancellationToken cancellationToken)
    {
        var entity = await _context.Bookings.FindAsync(new object[] { request.BookingId }, cancellationToken);
        if (entity is null)
        {
            return Errors.Booking.NotFound;
        }

        entity.CustomerId = request.CustomerId;
        entity.SellerId = request.SellerId;
        entity.ProductId = request.ProductId;
        entity.Location = request.Location;
        entity.StartTime = request.StartTime;
        entity.EndTime = request.EndTime;
        entity.NumberOfGuests = request.NumberOfGuests;
        entity.RoomType = request.RoomType;
        entity.Notes = request.Notes;

        await _context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}