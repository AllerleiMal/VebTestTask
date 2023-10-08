using VebTestTask.Filter;
using VebTestTask.Models;

namespace VebTestTask.Data;

public class PaginatedUsersParams
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public bool AscendingOrder { get; set; } = true;
    public string SortBy { get; set; } = "Id";
    public string NameStartsWith { get; set; } = "";
    public string EmailStartsWith { get; set; } = "";
    public int MinAge { get; set; } = 0;
    public int MaxAge { get; set; } = int.MaxValue;
    public List<int> RoleIds { get; set; } = new();

    public static bool TryParseRoleIds(string input, out List<int> parsedIds)
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

    public static bool TryParseUserProperty(Type type, string name, out string properName)
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