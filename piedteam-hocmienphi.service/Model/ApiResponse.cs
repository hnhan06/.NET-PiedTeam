namespace piedteam_hocmienphi.service.Model;

public class ApiResponse
{
    public bool IsSuccess { get; set; }
    public required string Message { get; set; }    // Login thành công || Login thất bại
    public object? Data { get; set; }
    public object? Errors { get; set; }
    public object? TraceId { get; set; }
    // Mỗi request sẽ được định danh 1 traceId
    // TraceId dùng để phân biệt các request với nhau
    public DateTime TimestampUtc { get; set; }
}

public static class ResponseBuilder
{
    public static ApiResponse SuccessResponse(object? data, string message, string? traceId = null)
    {
        return new ApiResponse()
        {
            IsSuccess = true,
            Message = message,
            Data = data,
            TraceId = traceId,
            TimestampUtc = DateTime.UtcNow
        };
    }

    public static ApiResponse ErrorResponse(object? errors, string message, string? traceId = null)
    {
        return new ApiResponse()
        {
            IsSuccess = true,
            Message = message,
            Errors = errors,
            TraceId = traceId,
            TimestampUtc = DateTime.UtcNow
        };
    }
}