using piedteam_hocmienphi.service.Model;

namespace piedteam_hocmienphi.api.Middlewares;

public class GlobalExceptionHandlerMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
            // tạo ra 1 cái try catch
            // sau đó chờ kết quả của request này trả ra
            // req lúc này bị next rồi, req sẽ đi xuống controller, service, repo
            // vì await ở đây nên mình có thể nhận đc response của request
            // response của request ở đây có 2 trường hợp:
                // Data bình thường
                // 1 Exception Error
                    // catch nó
            
            // Tại sao phải có Global Exception
                // Hạn chế duplicate logic try-catch
                // Không thể cover hết tất cả các Exception đc
                // Tránh bị crash app vì nếu có 1 Exception ko đc bắt thì app nổ
        }
        catch (Exception ex)
        {
            var statusCode = MapStatusCode(ex);
            var errorMessage = GetErrorMessage(ex, statusCode);
            var detailError = BuildErrorDetail(ex);
            
            // HttpContext context: context này đại diện cho req và res
            // nếu muốn độ lại status code của res thì tương tác với context
            
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var response = ResponseBuilder.ErrorResponse(detailError, errorMessage, context.TraceIdentifier);
            
            await context.Response.WriteAsJsonAsync(response);
        }
    }
    
    // Có được Exception, thì mình còn 2 thứ để trả ra
        // Code: 400, 422, 500
        // Message của Exception

    private static int MapStatusCode(Exception ex)
    {
        return ex switch
        {
            ArgumentException => StatusCodes.Status400BadRequest,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            KeyNotFoundException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };
    }
    
    private static string GetErrorMessage(Exception ex, int statusCode)
    {
        return statusCode >= 500 ? "Unexpected error occured" : ex.Message;
    }

    private static object? BuildErrorDetail(Exception ex)
    {
        return new
        {
            detail = ex.Message,
            exceptionType = ex.GetType().FullName,
            innerDetail = ex.InnerException?.Message,
            rootCauseDetail = ex.GetBaseException().Message
        };
    }
}