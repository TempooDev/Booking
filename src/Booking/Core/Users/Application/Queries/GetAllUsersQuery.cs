using Booking.Core.Users.Domain.Entities;
using Booking.Core.Users.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace Booking.Core.Users.Application.Queries;

public record GetAllUsersQuery : IRequest<ErrorOr<IEnumerable<User>>>;

public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, ErrorOr<IEnumerable<User>>>
{
    private readonly IUserRepository _userRepository;

    public GetAllUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ErrorOr<IEnumerable<User>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllUsersAsync();
        return users.Any()
            ? users.ToList()
            : new List<Error> { Error.NotFound("NoUsersFound", "No users were found.") };
    }
}