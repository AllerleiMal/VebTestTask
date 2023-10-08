using VebTestTask.Models;

namespace VebTestTask.Data.Repositories;

public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<Tuple<IEnumerable<User>, long>> GetPaginatedUsersAsync(PaginatedUsersParams userParams);
    Task<User?> GetUserByIdAsync(int userId);
    Task InsertUserAsync(User user);
    Task DeleteUserAsync(int userId);
    Task UpdateUserAsync(User user);
    Task SaveChangesAsync();
}