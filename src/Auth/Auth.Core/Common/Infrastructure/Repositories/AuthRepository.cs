using Auth.Core.Common.Domain.Entities;
using Auth.Core.Common.Interfaces;
using Auth.Core.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Auth.Core.Common.Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AuthDbContext _context;

        public AuthRepository(AuthDbContext context)
        {
            _context = context;
        }

        public async Task AddUserAsync(User user, CancellationToken cancellationToken = default)
        {
            // Ensure the User class being used here matches the DbSet<User> in AuthDbContext
            await _context.Users.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<User?> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            // Ensure the User class being used here matches the DbSet<User> in AuthDbContext
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }
    }
}
