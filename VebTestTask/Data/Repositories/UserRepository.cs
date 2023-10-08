using System.Linq.Expressions;
using System.Reflection;
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
    
    private static Expression<Func<T, T1>> CreateSelectorExpression<T, T1>(string propertyName)
    {
        var parameterExpression = Expression.Parameter(typeof(T));
        return (Expression<Func<T, T1>>)Expression.Lambda(Expression.PropertyOrField(parameterExpression, propertyName),
            parameterExpression);
    }

    private IQueryable<User> SortUsersBy(IQueryable<User> users, string property, bool ascendingOrder)
    {
        Console.WriteLine(property);
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
                    ? users.OrderBy(user => user.Roles.Count).ThenBy(user => user.Roles.Select(role => role.Id).Sum())
                    : users.OrderByDescending(user => user.Roles.Count).ThenByDescending(user => user.Roles.Select(role => role.Id).Sum());
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
        var allUsers = _context.Users.Where(user => user.Age >= userParams.MinAge && user.Age <= userParams.MaxAge);

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
            var chosenRoles = await _context.Roles.Where(role => userParams.RoleIds.Contains(role.Id)).ToListAsync();
            allUsers = allUsers.Where(user => user.Roles.Count >= chosenRoles.Count && user.Roles.All(x => chosenRoles.Contains(x)));
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