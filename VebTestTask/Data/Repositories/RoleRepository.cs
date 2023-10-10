using Microsoft.EntityFrameworkCore;
using VebTestTask.Models;

namespace VebTestTask.Data.Repositories;

public class RoleRepository : IRoleRepository
{
    private readonly UserContext _context;

    public RoleRepository(UserContext context)
    {
        _context = context;
    }

    public async Task<Role?> GetRoleByIdAsync(int id)
    {
        var targetRole = await _context.Roles.SingleOrDefaultAsync(r => r.Id == id);
        if (targetRole is null)
        {
            return await Task.FromResult<Role?>(null);
        }

        return await Task.FromResult<Role?>(targetRole);
    }

    public async Task<IEnumerable<Role>> GetRolesAsync()
    {
        return await _context.Roles.ToListAsync();
    }
}