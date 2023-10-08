namespace VebTestTask.Wrapper;

public class PagedResponse<T> : Response<T>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
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