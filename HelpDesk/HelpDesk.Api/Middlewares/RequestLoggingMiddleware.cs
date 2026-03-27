using System.Diagnostics;
using System.Security.Claims;

namespace HelpDesk.Api.Middlewares
{
    public class RequestLoggingMiddleware : IMiddleware
    {
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(ILogger<RequestLoggingMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var watch = Stopwatch.StartNew();

            _logger.LogInformation("Request: {Method} {Path} {UserRole}",
                context.Request.Method,
                context.Request.Path,
                (context.User.FindFirst(ClaimTypes.Role)?.Value ?? "None")
            );

            await next.Invoke( context );

            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;

            _logger.LogInformation("Response: {StatusCode} {elapsedMs}ms",
                context.Response.StatusCode,
                elapsedMs
            );
        }
    }
}
