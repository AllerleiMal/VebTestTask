namespace VebTestTask.Wrapper;

/// <summary>
/// Generic response wrapper
/// </summary>
/// <typeparam name="T">Type of value returned to the client</typeparam>
public class Response<T>
{
    public Response()
    {
    }
    public Response(T data)
    {
        Succeeded = true;
        Message = string.Empty;
        Errors = null;
        Data = data;
    }
    
    /// <summary>
    /// Wrapped request return data
    /// </summary>
    public T Data { get; set; }
    
    /// <summary>
    /// Flag of successful request processing
    /// </summary>
    public bool Succeeded { get; set; }
    
    /// <summary>
    /// Errors encountered during implementation
    /// </summary>
    public string[] Errors { get; set; }
    
    /// <summary>
    /// Explanatory message to client
    /// </summary>
    public string Message { get; set; }
}