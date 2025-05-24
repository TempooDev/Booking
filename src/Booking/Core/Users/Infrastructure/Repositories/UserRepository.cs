using Booking.Core.Common.Infrastructure.Persistence;
using Booking.Core.Users.Domain.Entities;
using Booking.Core.Users.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Booking.Core.Users.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddUserAsync(User user)
    {
        await _context.Set<User>().AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        return await _context.Set<User>().FindAsync(id);
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _context.Set<User>().ToListAsync();
    }

    public async Task DeleteUserAsync(User user)
    {
        _context.Set<User>().Remove(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUserAsync(User user)
    {
        _context.Set<User>().Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
       return await _context.Set<User>().FirstOrDefaultAsync(u => u.Email == email);
    }
}