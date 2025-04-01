using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Booking.Booking.Application.Booking.Domain;
using Booking.Booking.Application.Common;
using Booking.Booking.Application.Common.Infrastructure.Persistence;
using Booking.Shared.Common;

using ErrorOr;

using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Booking.Booking.Application.Booking.Application.Commands;

[ApiController]
[Route($"{ApiPaths.Root}/[controller]")]
[ApiExplorerSettings(GroupName = ApiPaths.Booking)]
public class CreateBookingController : ApiControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateBookingCommand command)
    {
        var result = await Mediator.Send(command);
        return result.Match(
            id => Ok(id),
            Problem);
    }
}

public record CreateBookingCommand(Guid CustomerId, Guid SellerId, Guid ProductId, string? Location, DateTime StartTime, DateTime EndTime, int NumberOfGuests, string? RoomType, string? Notes) : IRequest<ErrorOr<Guid>>;

internal sealed class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingCommandValidator()
    {
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

internal sealed class CreateBookingCommandHandler(ApplicationDbContext context) : IRequestHandler<CreateBookingCommand, ErrorOr<Guid>>
{
    private readonly ApplicationDbContext _context = context;
    public async Task<ErrorOr<Guid>> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        var entity = new BookingItem
        {
            CustomerId = request.CustomerId,
            SellerId = request.SellerId,
            ProductId = request.ProductId,
            Location = request.Location,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            NumberOfGuests = request.NumberOfGuests,
            RoomType = request.RoomType,
            Notes = request.Notes,
        };
        _context.Bookings.Add(entity);
        await _context.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }
}



