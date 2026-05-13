using System.Net;
using System.Text.Json;

namespace ApiProject.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _env;

        public ExceptionMiddleware(
            RequestDelegate next,
            ILogger<ExceptionMiddleware> logger,
            IWebHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Unhandled exception. Path: {Path}",
                    context.Request.Path);

                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            context.Response.StatusCode = exception switch
            {
                ArgumentException => (int)HttpStatusCode.BadRequest,
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                _ => (int)HttpStatusCode.InternalServerError
            };

            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = GetMessage(context.Response.StatusCode),
                Details = _env.IsDevelopment() ? exception.Message : null
            };

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            return context.Response.WriteAsync(
                JsonSerializer.Serialize(response, options));
        }

        private static string GetMessage(int statusCode)
        {
            return statusCode switch
            {
                400 => "בקשה לא תקינה",
                401 => "אין הרשאה לפעולה זו",
                404 => "המשאב לא נמצא",
                _ => "אירעה שגיאה בשרת. אנא נסה שוב מאוחר יותר"
            };
        }
    }
}
