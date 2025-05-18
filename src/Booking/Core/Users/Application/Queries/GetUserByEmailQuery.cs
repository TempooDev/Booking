using Booking.Core.Users.Domain.Entities;
using Booking.Core.Users.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace Booking.Core.Users.Application.Queries;

public record GetUserByEmailQuery(string Email) : IRequest<ErrorOr<User>>;

public class GetUserByEmailQueryHandler : IRequestHandler<GetUserByEmailQuery, ErrorOr<User>>
{
    private readonly IUserRepository _userRepository;

    public GetUserByEmailQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ErrorOr<User>> Handle(GetUserByEmailQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByEmailAsync(request.Email);
        if (user is null)
        {
            return Error.NotFound("User.NotFound", "User not found.");
        }

        return user;
    }
}