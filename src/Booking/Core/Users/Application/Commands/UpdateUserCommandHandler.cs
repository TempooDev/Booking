using Booking.Core.Users.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace Booking.Core.Users.Application.Commands;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, ErrorOr<Unit>>
{
    private readonly IUserRepository _userRepository;

    public UpdateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ErrorOr<Unit>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByIdAsync(request.Id);

        if (user == null)
        {
            return Error.NotFound("User.NotFound", "User not found.");
        }

        user.FirstName = request.FirstName;
        user.Name = request.Name;
        user.LastName = request.LastName;
        user.Email = request.Email;
        user.Role = request.Role;

        await _userRepository.UpdateUserAsync(user);
        return Unit.Value;
    }
}