using Booking.Core.Users.Domain.Entities;
using Booking.Core.Users.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace Booking.Core.Users.Application.Queries;

public record GetUsersByRoleQuery(UserRole Role) : IRequest<ErrorOr<IEnumerable<User>>>;

public class GetUsersByRoleQueryHandler : IRequestHandler<GetUsersByRoleQuery, ErrorOr<IEnumerable<User>>>
{
    private readonly IUserRepository _userRepository;

    public GetUsersByRoleQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ErrorOr<IEnumerable<User>>> Handle(GetUsersByRoleQuery request, CancellationToken cancellationToken)
    {
        var users = await _userRepository.GetAllUsersAsync();
        var filteredUsers = users.Where(user => user.Role == request.Role);

        // Fix: Use the correct method to wrap the result in an ErrorOr object
        return filteredUsers.ToList();
    }
}