namespace VebTestTask.Wrapper;

/// <summary>
/// Wrapper for responses from methods implementing pagination of returned data
/// </summary>
/// <typeparam name="T">Type of value returned to the client</typeparam>
public class PagedResponse<T> : Response<T>
{
    /// <summary>
    /// Current page number
    /// </summary>
    public int PageNumber { get; set; }
    /// <summary>
    /// Current page size
    /// </summary>
    public int PageSize { get; set; }
    /// <summary>
    /// Total amount of sample records after applying filtering
    /// </summary>
    public long TotalRecords { get; set; }
    
    public PagedResponse(T data, int pageNumber, int pageSize, long totalRecords) : base(data)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalRecords = totalRecords;
    }

    public PagedResponse(): base(default)
    {
        PageNumber = 1;
        PageSize = 10;
    }
}