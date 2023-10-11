using VebTestTask.Models;

namespace VebTestTask.Data.Repositories;

/// <summary>
/// Interface for Repository of Users
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Asynchronously returns all Users from DbSet
    /// </summary>
    /// <returns>IEnumerable of found Users, empty list if no Users found</returns>
    Task<IEnumerable<User>> GetAllUsersAsync();
    
    /// <summary>
    /// Asynchronously returns the selected page of Users after applying sorting and filtering
    /// </summary>
    /// <param name="userParams">Specified pagination, filtering and sorting parameters</param>
    /// <returns>
    /// A task that represents the asynchronous operation. Task contains Tuple of two elements; <para/>
    /// First element is IEnumerable of Users on selected page after filtering and sorting.<para/>
    /// Second element is total amount of records selected before pagination
    /// </returns>
    Task<Tuple<IEnumerable<User>, long>> GetPaginatedUsersAsync(PaginatedUsersParams userParams);
    
    /// <summary>
    /// Asynchronously returns the only User that has entered ID or null if no such User exist 
    /// </summary>
    /// <param name="userId">Unique ID of the User</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the single User that has entered ID,
    /// or null if no such element is found.
    /// </returns>
    Task<User?> GetUserByIdAsync(int userId);
    
    /// <summary>
    /// Asynchronously returns the only User that has entered Email or null if no such User exist 
    /// </summary>
    /// <param name="email">Unique Email of the User</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the single User that has entered Email,
    /// or null if no such element is found.
    /// </returns>
    Task<User?> GetUserByEmailAsync(string email);
    
    /// <summary>
    /// Asynchronously inserts the specified User
    /// </summary>
    /// <param name="user">User to insert</param>
    /// <returns>
    /// A task that represents the asynchronous operation. Task contains inserted User.
    /// </returns>
    Task<User?> InsertUserAsync(User user);
    
    /// <summary>
    /// Asynchronously deletes the User with specified ID
    /// </summary>
    /// <param name="userId">ID of User to delete</param>
    /// <returns>
    /// A task that represents the asynchronous operation.<para/>
    /// Task contains null if User with such ID does not exist.<para/>
    /// Otherwise task contains deleted User.
    /// </returns>
    Task<User?> DeleteUserAsync(int userId);
    
    /// <summary>
    /// Asynchronously updates the specified User in database
    /// </summary>
    /// <param name="user">Updated User</param>
    /// <returns>
    /// A task that represents the asynchronous operation.<para/>
    /// If specified User is not tracked in DbContext, method passed without any database changes.<para/>
    /// Otherwise changes are pushed to the database.
    /// </returns>
    Task UpdateUserAsync(User user);
    
    /// <summary>
    /// Asynchronously updates the specified User by adding a new Role
    /// </summary>
    /// <param name="user">User to update</param>
    /// <param name="role">Role to add</param>
    /// <returns>
    /// A task that represents the asynchronous operation.<para/>
    /// If specified User already has this role, method passed without any database changes<para/>
    /// Otherwise changes are pushed to the database.
    /// </returns>
    Task<User?> AddNewRoleForUserAsync(User user, Role role);

    /// <summary>
    /// Asynchronously returns the only User with entered email and name or null if no such User exist 
    /// </summary>
    /// <param name="name">Name of the User</param>
    /// <param name="email">Unique email address of the User</param>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains the single User with entered email
    ///  and name, or null if no such element is found.
    /// </returns>
    Task<User?> GetUserByNameAndEmailAsync(string name, string email);
    
    /// <summary>
    /// Asynchronously save database changes
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous save operation.
    /// The task result contains the number of state entries written to the database.
    /// </returns>
    Task SaveChangesAsync();
}