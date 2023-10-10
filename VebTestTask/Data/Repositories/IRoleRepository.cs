using VebTestTask.Models;

namespace VebTestTask.Data.Repositories;

public interface IRoleRepository
{
    Task<Role?> GetRoleByIdAsync(int id);
    Task<IEnumerable<Role>> GetRolesAsync();
}