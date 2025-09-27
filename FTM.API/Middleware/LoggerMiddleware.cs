using System.Diagnostics;
using System.Text;
namespace FTM.API.Middleware
{
    public class LoggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggerMiddleware> _logger;

        public LoggerMiddleware(RequestDelegate next, ILogger<LoggerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();

            // Log Request
            context.Request.EnableBuffering(); // allow multiple reads

            string requestBody = "";
            if (context.Request.ContentLength > 0 &&
                context.Request.Body.CanRead &&
                context.Request.ContentType != null &&
                context.Request.ContentType.Contains("application/json", StringComparison.OrdinalIgnoreCase))
            {
                using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
                requestBody = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0; // rewind
            }

            _logger.LogInformation("Request {method} {path}\nHeaders: {@headers}\nBody: {body}",
                context.Request.Method,
                context.Request.Path,
                context.Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString()),
                string.IsNullOrWhiteSpace(requestBody) ? "(empty)" : requestBody);

            // Capture Response
            var originalBodyStream = context.Response.Body;
            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await _next(context);

            stopwatch.Stop();

            // Read response
            context.Response.Body.Seek(0, SeekOrigin.Begin);
            string responseText = await new StreamReader(context.Response.Body).ReadToEndAsync();
            context.Response.Body.Seek(0, SeekOrigin.Begin);

            _logger.LogInformation("Response {statusCode} ({elapsed} ms)\nBody: {body}",
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds,
                string.IsNullOrWhiteSpace(responseText) ? "(empty)" : responseText);

            // Copy response back to original stream
            await responseBody.CopyToAsync(originalBodyStream);
        }
    }
}
