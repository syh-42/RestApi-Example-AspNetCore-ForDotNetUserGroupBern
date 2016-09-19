using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace RestApi.Middleware
{
    public class RequestLoggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public RequestLoggerMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            _next = next;
            _logger = loggerFactory.CreateLogger<RequestLoggerMiddleware>();
        }

        public async Task Invoke(HttpContext context)
        {
            _logger.LogInformation($"\n\r---------------------------------------\n\rRequest auf {context.Request.Path} wurde gestartet\n\r---------------------------------------");
            await _next.Invoke(context);
            _logger.LogInformation("\n\r---------------------------------------\n\rAnfrage fertig bearbeitet.\n\r---------------------------------------");
        }
    }
}