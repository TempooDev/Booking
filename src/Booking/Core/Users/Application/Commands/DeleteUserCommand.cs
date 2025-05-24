using ErrorOr;
using MediatR;

namespace Booking.Core.Users.Application.Commands;

public record DeleteUserCommand(Guid Id) : IRequest<ErrorOr<Unit>>;