using Microsoft.EntityFrameworkCore;
using VebTestTask.Models;

namespace VebTestTask.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly UserContext _context;

    public UserRepository(UserContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _context.Users.Include(user => user.Roles).ToListAsync();
    }

    public Task<IEnumerable<User>> GetPaginatedUsersAsync(GetPaginatedUsersParams userParams)
    {
        throw new NotImplementedException();
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _context.Users.Where(user => user.Id == userId).Include(user => user.Roles).FirstOrDefaultAsync();
    }

    public async Task InsertUserAsync(User user)
    {
        _context.Add(user);
        await SaveChangesAsync();
    }

    public async Task DeleteUserAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user is null)
        {
            return;
        }
        
        _context.Remove(user);
        await SaveChangesAsync();
    }

    public async Task UpdateUserAsync(User user)
    {
        _context.Entry(user).State = EntityState.Modified;
        await SaveChangesAsync();
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}