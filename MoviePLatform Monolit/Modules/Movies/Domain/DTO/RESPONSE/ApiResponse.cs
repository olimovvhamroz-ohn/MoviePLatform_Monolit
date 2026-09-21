
/// <summary>
/// Универсальный класс для обёртки API ответов
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }
    public int StatusCode { get; set; }

    public static ApiResponse<T> SuccessResponse(T data,
        string message = "Success", 
        int statusCode = 200)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data,
            StatusCode = statusCode
        };
    }

    public static ApiResponse<T> ErrorResponse(
        string message, 
        List<string>? errors = null,
        int statusCode = 400)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors ?? new List<string> { message },
            StatusCode = statusCode
        };
    }

    public static ApiResponse<T> NotFoundResponse(string message = "Resource not found")
    {
        return ErrorResponse(message, statusCode: 404);
    }
}

public class ApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public List<string>? Errors { get; set; }
    public int StatusCode { get; set; }

    public static ApiResponse SuccessResponse(string message = "Success", int statusCode = 200)
    {
        return new ApiResponse
        {
            Success = true,
            Message = message,
            StatusCode = statusCode
        };
    }

    public static ApiResponse ErrorResponse(string message, List<string>? errors = null, int statusCode = 400)
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            Errors = errors ?? new List<string> { message },
            StatusCode = statusCode
        };
    }
}


public class PaginatedResponse<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (TotalCount + PageSize - 1) / PageSize;
    public bool HasNextPage => PageNumber < TotalPages;
    public bool HasPreviousPage => PageNumber > 1;
}


