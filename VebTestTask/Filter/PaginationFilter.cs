namespace VebTestTask.Filter;

/// <summary>
/// Query parameters wrapper for paginated GET request
/// </summary>
public class PaginationFilter
{
    /// <summary>
    /// Chosen page number
    /// </summary>
    public int PageNumber { get; set; }
    /// <summary>
    /// Chosen page size
    /// </summary>
    public int PageSize { get; set; }
    /// <summary>
    /// Chosen ordering target
    /// </summary>
    public string OrderBy { get; set; }
    /// <summary>
    /// Flag for ascending sort order
    /// </summary>
    public bool OrderAsc { get; set; }
    /// <summary>
    /// User name start mask
    /// </summary>
    public string NameStartsWith { get; set; }
    /// <summary>
    /// User email start mask
    /// </summary>
    public string EmailStartsWith { get; set; }
    /// <summary>
    /// Age lower bound
    /// </summary>
    public int MinAge { get; set; }
    /// <summary>
    /// Age upper bound
    /// </summary>
    public int MaxAge { get; set; }
    /// <summary>
    /// Roles' IDS with comma as delimiter
    /// </summary>
    public string RoleIds { get; set; }

    public PaginationFilter()
    {
        PageNumber = 1;
        PageSize = 10;
        OrderBy = "Id";
        OrderAsc = true;
        NameStartsWith = "";
        EmailStartsWith = "";
        MinAge = 0;
        MaxAge = int.MaxValue;
        RoleIds = "";
    }

    public PaginationFilter(int pageNumber, int pageSize, string orderBy, bool orderAsc, string nameStartsWith, string emailStartsWith,
        int minAge, int maxAge, string roleIds)
    {
        OrderBy = orderBy;
        OrderAsc = orderAsc;
        NameStartsWith = nameStartsWith;
        EmailStartsWith = emailStartsWith;
        RoleIds = roleIds;
        MinAge = minAge;
        MaxAge = maxAge;
        PageNumber = pageNumber < 1 ? 1 : pageNumber;
        PageSize = pageSize > 10 ? 10 : pageSize;
    }
}