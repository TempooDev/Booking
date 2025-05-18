using Booking.Core.Users.Domain.Entities;

namespace Booking.Core.Users.Domain.Repositories;

public interface IUserRepository
{
    Task AddUserAsync(User user);
    Task<User?> GetUserByIdAsync(Guid id);
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task DeleteUserAsync(User user);
    Task UpdateUserAsync(User user);
}