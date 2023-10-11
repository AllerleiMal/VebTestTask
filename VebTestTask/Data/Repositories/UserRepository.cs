using System.Linq.Expressions;
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
        return await _context.Users
            .Include(user => user.Roles)
            .ToListAsync();
    }

    /// <summary>
    /// Custom key selector for applying SortBy to IQueryable
    /// </summary>
    /// <param name="propertyName">Proper name of property to be selected</param>
    /// <typeparam name="T">Type of entity that contains required property</typeparam>
    /// <typeparam name="T1">Type of required property</typeparam>
    /// <returns>Key selector lambda expression</returns>
    private static Expression<Func<T, T1>> CreateSelectorExpression<T, T1>(string propertyName)
    {
        var parameterExpression = Expression.Parameter(typeof(T));
        return (Expression<Func<T, T1>>)Expression.Lambda(Expression.PropertyOrField(parameterExpression, propertyName),
            parameterExpression);
    }

    /// <summary>
    /// Applies OrderBy on specified IQueryable of Users
    /// </summary>
    /// <param name="users">Specified users</param>
    /// <param name="property">Ordering target property</param>
    /// <param name="ascendingOrder">Flag of ascending order</param>
    /// <returns>Ordered IQueryable of Users</returns>
    private IQueryable<User> SortUsersBy(IQueryable<User> users, string property, bool ascendingOrder)
    {
        IQueryable<User> sortedUsers;
        switch (property)
        {
            case "id" or "age":
            {
                var customKeySelector = CreateSelectorExpression<User, int>(property);
                sortedUsers = ascendingOrder
                    ? users.OrderBy(customKeySelector)
                    : users.OrderByDescending(customKeySelector);
                break;
            }
            case "roles":
            {
                sortedUsers = ascendingOrder
                    ? users
                        .OrderBy(user => user.Roles.Count)
                        .ThenBy(user => user.Roles.Select(role => role.Id).Sum())
                    : users
                        .OrderByDescending(user => user.Roles.Count)
                        .ThenByDescending(user => user.Roles.Select(role => role.Id).Sum());
                break;
            }
            default:
            {
                var customKeySelector = CreateSelectorExpression<User, string>(property);
                sortedUsers = ascendingOrder
                    ? users.OrderBy(customKeySelector)
                    : users.OrderByDescending(customKeySelector);
                break;
            }
        }

        return sortedUsers;
    }

    public async Task<Tuple<IEnumerable<User>, long>> GetPaginatedUsersAsync(PaginatedUsersParams userParams)
    {
        var allUsers = _context.Users
            .Where(user => user.Age >= userParams.MinAge && user.Age <= userParams.MaxAge);

        if (!string.IsNullOrEmpty(userParams.NameStartsWith))
        {
            allUsers = allUsers.Where(user => user.Name.ToLower().StartsWith(userParams.NameStartsWith.ToLower()));
        }

        if (!string.IsNullOrEmpty(userParams.EmailStartsWith))
        {
            allUsers = allUsers.Where(user => user.Email.ToLower().StartsWith(userParams.EmailStartsWith.ToLower()));
        }

        allUsers = allUsers.Include(user => user.Roles);

        if (userParams.RoleIds.Any())
        {
            var chosenRoles = await _context.Roles
                .Where(role => userParams.RoleIds.Contains(role.Id))
                .ToListAsync();
            allUsers = allUsers.Where(user =>
                user.Roles.Count >= chosenRoles.Count && user.Roles.All(x => chosenRoles.Contains(x)));
        }


        allUsers = SortUsersBy(allUsers, property: userParams.SortBy, userParams.AscendingOrder);

        var totalRecords = await allUsers.LongCountAsync();

        var pagedUsers = await allUsers
            .Skip((userParams.PageNumber - 1) * userParams.PageSize)
            .Take(userParams.PageSize)
            .ToListAsync();

        return new Tuple<IEnumerable<User>, long>(pagedUsers, totalRecords);
    }

    public async Task<User?> GetUserByIdAsync(int userId)
    {
        return await _context.Users
            .Where(user => user.Id == userId)
            .Include(user => user.Roles)
            .SingleOrDefaultAsync();
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users
            .Where(user => user.Email.Equals(email))
            .Include(user => user.Roles)
            .SingleOrDefaultAsync();
    }

    public async Task<User?> InsertUserAsync(User user)
    {
        var requiredRoles = _context.Roles
            .ToList()
            .Where(role => user.Roles.Select(r => r.Id).Contains(role.Id))
            .ToList();
        
        user.Roles.Clear();
        foreach (var role in requiredRoles)
        {
            user.Roles.Add(role);
        }

        var insertedUser = _context.Add(user);
        await SaveChangesAsync();

        return insertedUser.Entity;
    }

    public async Task<User?> DeleteUserAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user is null)
        {
            return await Task.FromResult<User?>(null);
        }

        _context.Remove(user);
        await SaveChangesAsync();

        return await Task.FromResult<User?>(user);
    }

    public async Task UpdateUserAsync(User user)
    {
        var changedUser = await GetUserByIdAsync(user.Id);
        if (changedUser is null)
        {
            return;
        }

        var newRoles = _context.Roles
            .ToList()
            .Where(role => user.Roles.Any(r => r.Id == role.Id))
            .ToList();
        
        user.Roles.Clear();
        foreach (var role in newRoles)
        {
            user.Roles.Add(role);
        }

        changedUser.ApplyChangesExceptId(user);
        await SaveChangesAsync();
    }

    public async Task<User?> AddNewRoleForUser(User user, Role role)
    {
        if (user.Roles.Contains(role))
        {
            return await Task.FromResult<User?>(null);
        }

        user.Roles.Add(role);
        await SaveChangesAsync();

        return user;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}