using NewsAggregator.Exceptions;
using System.Net;
using System.Text.Json;

namespace NewsAggregator.Middleware
{
    public class ExceptionHandlerMiddleware
    {
        private readonly ILogger<ExceptionHandlerMiddleware> _logger;
        private readonly RequestDelegate _next;

        public ExceptionHandlerMiddleware(ILogger<ExceptionHandlerMiddleware> logger, RequestDelegate next) => (_logger, _next) = (logger, next);

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception caught by {GetType().Name}");

                HttpResponse response = context.Response;
                response.ContentType = "application/json";
                response.StatusCode = ex switch
                {
                    HackerNewsResultsException => (int)HttpStatusCode.BadGateway,
                    _ => (int)HttpStatusCode.InternalServerError
                };

                string result = JsonSerializer.Serialize(new { message = ex?.Message });
                await response.WriteAsync(result);
            }
        }
    }
}
