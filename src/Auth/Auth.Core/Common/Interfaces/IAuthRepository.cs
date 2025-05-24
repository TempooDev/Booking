using Auth.Core.Common.Domain.Entities;

namespace Auth.Core.Common.Interfaces
{
    public interface IAuthRepository
    {
        Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task AddUserAsync(User user, CancellationToken cancellationToken = default);
    }
}
