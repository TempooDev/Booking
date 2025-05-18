using Booking.Core.Users.Domain.Entities;

using ErrorOr;
using MediatR;

namespace Booking.Core.Users.Application.Commands;

public record UpdateUserCommand(Guid Id, string FirstName, string LastName, string Name, string Email, UserRole Role) : IRequest<ErrorOr<Unit>>;