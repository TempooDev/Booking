using Booking.Core.Users.Domain.Entities;
using Booking.Core.Users.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace Booking.Core.Users.Application.Queries;

public record GetUserByIdQuery(Guid Id) : IRequest<ErrorOr<User>>;

public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, ErrorOr<User>>
{
    private readonly IUserRepository _userRepository;

    public GetUserByIdQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ErrorOr<User>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByIdAsync(request.Id);
        if (user is null)
        {
            return Error.NotFound("User.NotFound", "User not found.");
        }

        return user;
    }
}