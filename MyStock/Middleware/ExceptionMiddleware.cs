using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;

namespace MyStock.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");

                context.Response.ContentType = "application/problem+json";

                var problem = ex switch
                {
                    KeyNotFoundException => new ProblemDetails
                    {
                        Status = (int)HttpStatusCode.NotFound,
                        Title = "Not Found",
                        Detail = ex.Message,
                        Type = "https://httpstatuses.com/404"
                    },

                    InvalidOperationException => new ProblemDetails
                    {
                        Status = (int)HttpStatusCode.BadRequest,
                        Title = "Invalid Operation",
                        Detail = ex.Message,
                        Type = "https://httpstatuses.com/400"
                    },

                    ArgumentOutOfRangeException => new ProblemDetails
                    {
                        Status = (int)HttpStatusCode.BadRequest,
                        Title = "Invalid Argument",
                        Detail = ex.Message,
                        Type = "https://httpstatuses.com/400"
                    },

                    UnauthorizedAccessException => new ProblemDetails
                    {
                        Status = 401,
                        Title = "Unauthorized",
                        Detail = ex.Message,
                        Type = "https://httpstatuses.com/401"
                    },

                    _ => new ProblemDetails
                    {
                        Status = (int)HttpStatusCode.InternalServerError,
                        Title = "Internal Server Error",
                        Detail = "An unexpected error occurred.",
                        Type = "https://httpstatuses.com/500"
                    }
                };

                context.Response.StatusCode = problem.Status ?? 500;

                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                await context.Response.WriteAsync(JsonSerializer.Serialize(problem, options));
            }
        }
    }
}
