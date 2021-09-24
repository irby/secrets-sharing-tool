using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace SecretsSharingTool.Api.Middleware
{
    public sealed class IpAddressLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private ILogger<IpAddressLoggingMiddleware> _logger;

        public IpAddressLoggingMiddleware(RequestDelegate next, ILogger<IpAddressLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var ipAddress = context.Connection.RemoteIpAddress;
            var method = context.Request.Method;
            var path = context.Request.Path;

            _logger.LogInformation($"Request received\t{ipAddress}\t{method}\t{path}");

            await _next.Invoke(context);

            var responseStatusCode = context.Response.StatusCode;
            
            _logger.LogInformation($"Request complete\t{ipAddress}\t{method}\t{path}\t{responseStatusCode}");
        }
    }
}