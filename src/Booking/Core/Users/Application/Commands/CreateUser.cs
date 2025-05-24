using Booking.Core.Users.Domain.Entities;
using Booking.Core.Users.Domain.Repositories;
using ErrorOr;
using FluentValidation;
using MediatR;

namespace Booking.Core.Users.Application.Commands;

public record CreateUserCommand(
    string Name,
    string FirstName,
    string LastName,
    string Email,
    UserRole Role,
    string? PreferredPaymentMethod = null,
    string? StoreName = null,
    double? Rating = null) : IRequest<ErrorOr<Guid>>;

public sealed class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, ErrorOr<Guid>>
{
    private readonly IUserRepository _userRepository;

    public CreateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ErrorOr<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        User user;

        if (request.Role == UserRole.Buyer)
        {
            user = new Buyer
            {
                Name = request.Name,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PreferredPaymentMethod = request.PreferredPaymentMethod,
            };
        }
        else if (request.Role == UserRole.Seller)
        {
            user = new Seller
            {
                Name = request.Name,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                StoreName = request.StoreName,
                Rating = request.Rating,
            };
        }
        else
        {
            return Error.Validation("InvalidRole", "The specified role is invalid.");
        }

        await _userRepository.AddUserAsync(user);
        return user.Id;
    }
}

internal sealed class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(v => v.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(v => v.LastName)
            .NotEmpty()
            .MaximumLength(100);
        RuleFor(v => v.Email)
            .NotEmpty()
            .EmailAddress();
        RuleFor(v => v.Role)
            .IsInEnum();
    }
}