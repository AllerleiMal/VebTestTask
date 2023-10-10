using VebTestTask.Models;

namespace VebTestTask.Data.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<Tuple<IEnumerable<User>, long>> GetPaginatedUsersAsync(PaginatedUsersParams userParams);
    Task<User?> GetUserByIdAsync(int userId);
    Task<User?> GetUserByEmailAsync(string email);
    Task<User?> InsertUserAsync(User user);
    Task<User?> DeleteUserAsync(int userId);
    Task UpdateUserAsync(User user);
    Task<User?> AddNewRoleForUser(User user, Role role);
    Task SaveChangesAsync();
}