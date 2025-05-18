using Booking.Core.Users.Domain.Entities;
using Booking.Core.Users.Domain.Repositories;
using ErrorOr;
using FluentValidation;
using MediatR;

namespace Booking.Core.Users.Application.Commands;

public record CreateUser(
    string Name,
    string Email,
    UserRole Role,
    string? PreferredPaymentMethod = null,
    string? StoreName = null,
    double? Rating = null) : IRequest<ErrorOr<Guid>>;

public sealed class CreateUserCommandHandler : IRequestHandler<CreateUser, ErrorOr<Guid>>
{
    private readonly IUserRepository _userRepository;

    public CreateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ErrorOr<Guid>> Handle(CreateUser request, CancellationToken cancellationToken)
    {
        User user;

        if (request.Role == UserRole.Buyer)
        {
            user = new Buyer
            {
                Name = request.Name,
                Email = request.Email,
                PreferredPaymentMethod = request.PreferredPaymentMethod,
            };
        }
        else if (request.Role == UserRole.Seller)
        {
            user = new Seller
            {
                Name = request.Name,
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

internal sealed class CreateUserCommandValidator : AbstractValidator<CreateUser>
{
    public CreateUserCommandValidator()
    {
        RuleFor(v => v.Name)
            .NotEmpty()
            .MaximumLength(100);
        RuleFor(v => v.Email)
            .NotEmpty()
            .EmailAddress();
        RuleFor(v => v.Role)
            .IsInEnum();
    }
}