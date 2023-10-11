using VebTestTask.Models;

namespace VebTestTask.Data.Repositories;

/// <summary>
/// Interface for Repository of Roles
/// </summary>
public interface IRoleRepository
{
    /// <summary>
    /// Asynchronously returns the only Role that has entered ID or null if no such Role exist 
    /// </summary>
    /// <param name="id">Unique ID of the role</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the single Role that has entered ID,
    /// or null if no such element is found.
    /// </returns>
    Task<Role?> GetRoleByIdAsync(int id);
    /// <summary>
    /// Get all Roles
    /// </summary>
    /// <returns>IEnumerable of Roles found in DbSet</returns>
    Task<IEnumerable<Role>> GetRolesAsync();
}