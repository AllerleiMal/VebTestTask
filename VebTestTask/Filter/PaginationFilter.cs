namespace VebTestTask.Filter;

public class PaginationFilter
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public string OrderBy { get; set; }
    public bool OrderAsc { get; set; }
    public string NameStartsWith { get; set; }
    public string EmailStartsWith { get; set; }
    public int MinAge { get; set; }
    public int MaxAge { get; set; }
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