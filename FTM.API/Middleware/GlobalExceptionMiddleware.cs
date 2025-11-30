using FTM.API.Reponses;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Security;
using System.Text.Json;
using XAct;
using XAct.Messages;

namespace FTM.API.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var response = context.Response;
            response.ContentType = "application/json";
            var statusCode = HttpStatusCode.InternalServerError;
            string message = string.Empty;
            switch (ex)
            {
                case ArgumentException:
                case FormatException:
                case InvalidOperationException:
                    statusCode = HttpStatusCode.BadRequest;
                    break;

                case KeyNotFoundException:
                    statusCode = HttpStatusCode.NotFound;
                    break;

                case UnauthorizedAccessException:
                    statusCode = HttpStatusCode.Unauthorized;
                    break;

                case SecurityException:
                    statusCode = HttpStatusCode.Forbidden;
                    message = "Bạn không có quyền truy cập tài nguyên này.";
                    break;

                case DbUpdateConcurrencyException:
                    statusCode = HttpStatusCode.Conflict;
                    message = "Lỗi xảy ra trong quá trình lưu dữ liệu";
                    break;

                case DbUpdateException:
                    statusCode = HttpStatusCode.BadRequest;
                    message = "Lỗi xảy ra trong quá trình cập nhật dữ liệu.";
                    break;

                case TimeoutException:
                    statusCode = HttpStatusCode.RequestTimeout;
                    break;

                case HttpRequestException:
                    statusCode = HttpStatusCode.BadGateway;
                    message = "Lỗi xảy ra khi kết nối với dịch vụ khác.";
                    break;

                default:
                    message = "Đã xảy ra lỗi hệ thống.";
                    break;
            }

            response.StatusCode = (int)statusCode;
            var result = JsonSerializer.Serialize(new ApiError(string.IsNullOrEmpty(message) ? ex.Message : message, statusCode));
            return response.WriteAsync(result);
        }
    }
}
