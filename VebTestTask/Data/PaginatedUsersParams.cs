using VebTestTask.Filter;
using VebTestTask.Models;

namespace VebTestTask.Data;

/// <summary>
/// Wrapped parameters for Users pagination, sorting and filtering
/// </summary>
public class PaginatedUsersParams
{
    /// <summary>
    /// Chosen page number
    /// </summary>
    public int PageNumber { get; set; } = 1;
    /// <summary>
    /// Chosen size of each page
    /// </summary>
    public int PageSize { get; set; } = 10;
    /// <summary>
    /// Flag of ascending sort order
    /// </summary>
    public bool AscendingOrder { get; set; } = true;
    /// <summary>
    /// Sort target
    /// </summary>
    public string SortBy { get; set; } = "Id";
    public string NameStartsWith { get; set; } = "";
    public string EmailStartsWith { get; set; } = "";
    /// <summary>
    /// Lower age bound
    /// </summary>
    public int MinAge { get; set; } = 0;
    /// <summary>
    /// Upper age bound
    /// </summary>
    public int MaxAge { get; set; } = int.MaxValue;
    /// <summary>
    /// Chosen role ids restrictions
    /// </summary>
    public List<int> RoleIds { get; set; } = new();

    /// <summary>
    /// Tries to parse the string of ids with comma as delimiter to the List of ints 
    /// </summary>
    /// <param name="input">String of ids with comma as delimiter</param>
    /// <param name="parsedIds">Out parameter for parsing result</param>
    /// <returns>
    /// The flag of parsing success
    /// </returns>
    private static bool TryParseRoleIds(string input, out List<int> parsedIds)
    {
        parsedIds = new List<int>();
        if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input))
        {
            return true;
        }
        
        var parsedRoleIds = input.Split(',').Select(id => (Ok: int.TryParse(id, out int x), Value: x)).ToList();
        if (!parsedRoleIds.All(parsedIds => parsedIds.Ok))
        {
            return false;
        }

        parsedIds = parsedRoleIds.Select(x => x.Value).ToList();

        return true;
    }

    /// <summary>
    /// Tries to parse the entered name of property to proper type's property name
    /// </summary>
    /// <param name="type">Type where to search the property</param>
    /// <param name="name">Specified property name</param>
    /// <param name="properName">Out parameter for proper property name</param>
    /// <returns>
    /// The flag of parsing success
    /// </returns>
    private static bool TryParseUserProperty(Type type, string name, out string properName)
    {
        properName = "Id";
        if (string.IsNullOrEmpty(name) || string.IsNullOrWhiteSpace(name))
        {
            return true;
        }
        
        var userPropertiesNames = type.GetProperties().Select(x => x.Name.ToLower()).ToList();

        if (!userPropertiesNames.Contains(name.ToLower()))
        {
            return false;
        }
        
        properName = userPropertiesNames.First(propertyName => name.ToLower().Equals(propertyName.ToLower()));
        
        return true;
    }
    
    /// <summary>
    /// Validates the data from PaginationFilter, process it and creates PaginatedUserParams
    /// </summary>
    /// <param name="filter">Incoming request parameters</param>
    /// <returns>
    /// Returns task that contains parameters for users pagination from incoming request parameters, if data fulfils the requirements.<para/>
    /// Otherwise returns task that contains null.
    /// </returns>
    public static Task<PaginatedUsersParams?> GetParamsFromPaginationFilter(PaginationFilter filter)
    {
        var result = new PaginatedUsersParams
        {
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize,
            MinAge = filter.MinAge,
            MaxAge = filter.MaxAge,
            AscendingOrder = filter.OrderAsc
        };
        
        if (!TryParseRoleIds(filter.RoleIds, out List<int> roleIds))
        {
            return Task.FromResult<PaginatedUsersParams?>(null);
        }

        result.RoleIds = roleIds;

        if (!TryParseUserProperty(typeof(User), filter.OrderBy, out string sortTarget))
        {
            return Task.FromResult<PaginatedUsersParams?>(null);
        }

        result.SortBy = sortTarget;

        if (!string.IsNullOrEmpty(filter.EmailStartsWith) && !string.IsNullOrWhiteSpace(filter.EmailStartsWith))
        {
            result.EmailStartsWith = filter.EmailStartsWith;
        }
        
        if (!string.IsNullOrEmpty(filter.NameStartsWith) && !string.IsNullOrWhiteSpace(filter.NameStartsWith))
        {
            result.NameStartsWith = filter.NameStartsWith;
        }

        return Task.FromResult<PaginatedUsersParams?>(result);
    }
}