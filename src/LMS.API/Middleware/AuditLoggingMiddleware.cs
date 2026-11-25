using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LMS.API.Middleware;

public class AuditLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuditLoggingMiddleware> _logger;

    public AuditLoggingMiddleware(RequestDelegate next, ILogger<AuditLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var startTime = DateTime.UtcNow;
        var requestId = context.TraceIdentifier;
        var userId = context.User?.FindFirst("sub")?.Value ?? context.User?.FindFirst("userId")?.Value ?? "Anonymous";
        var userRole = context.User?.FindFirst("role")?.Value ?? context.User?.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value ?? "None";

        var originalBodyStream = context.Response.Body;

        using var requestBodyStream = new MemoryStream();
        using var responseBodyStream = new MemoryStream();

        try
        {
            await LogRequestAsync(context, requestId, userId, userRole, startTime, requestBodyStream);

            context.Response.Body = responseBodyStream;

            await _next(context);

            await LogResponseAsync(context, requestId, userId, userRole, startTime, responseBodyStream);

            await responseBodyStream.CopyToAsync(originalBodyStream);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in audit logging middleware for request {RequestId}", requestId);
            context.Response.Body = originalBodyStream;
            throw;
        }
    }

    private async Task LogRequestAsync(HttpContext context, string requestId, string userId, string userRole, DateTime startTime, MemoryStream requestBodyStream)
    {
        if (ShouldLogRequest(context.Request))
        {
            context.Request.EnableBuffering();

            var requestBody = string.Empty;
            if (context.Request.ContentLength > 0 && context.Request.Body.CanRead)
            {
                context.Request.Body.Position = 0;
                using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
                requestBody = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;
            }

            var auditLog = new
            {
                RequestId = requestId,
                Timestamp = startTime,
                UserId = userId,
                UserRole = userRole,
                HttpMethod = context.Request.Method,
                Path = context.Request.Path,
                QueryString = context.Request.QueryString.ToString(),
                IpAddress = context.Connection.RemoteIpAddress?.ToString(),
                UserAgent = context.Request.Headers.UserAgent.ToString(),
                RequestBody = requestBody,
                ContentType = context.Request.ContentType
            };

            _logger.LogInformation("AUDIT REQUEST: {AuditLog}", JsonConvert.SerializeObject(auditLog, Formatting.Indented));
        }
    }

    private async Task LogResponseAsync(HttpContext context, string requestId, string userId, string userRole, DateTime startTime, MemoryStream responseBodyStream)
    {
        if (ShouldLogRequest(context.Request))
        {
            var duration = DateTime.UtcNow - startTime;

            responseBodyStream.Position = 0;
            var responseBody = string.Empty;
            if (responseBodyStream.Length > 0)
            {
                using var reader = new StreamReader(responseBodyStream, Encoding.UTF8, leaveOpen: true);
                responseBody = await reader.ReadToEndAsync();
                responseBodyStream.Position = 0;
            }

            var auditLog = new
            {
                RequestId = requestId,
                Timestamp = DateTime.UtcNow,
                UserId = userId,
                UserRole = userRole,
                HttpMethod = context.Request.Method,
                Path = context.Request.Path,
                StatusCode = context.Response.StatusCode,
                DurationMs = duration.TotalMilliseconds,
                ResponseBody = responseBody,
                IpAddress = context.Connection.RemoteIpAddress?.ToString()
            };

            _logger.LogInformation("AUDIT RESPONSE: {AuditLog}", JsonConvert.SerializeObject(auditLog, Formatting.Indented));
        }
    }

    private static bool ShouldLogRequest(HttpRequest request)
    {
        var path = request.Path.Value?.ToLowerInvariant() ?? string.Empty;

        var excludedPaths = new[]
        {
            "/swagger",
            "/health",
            "/favicon.ico",
            "/api/health",
            "/hangfire"
        };

        return !excludedPaths.Any(p => path.StartsWith(p, StringComparison.OrdinalIgnoreCase));
    }
}