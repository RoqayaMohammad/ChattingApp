using Azure;
using ChattingApp.Errors;
using Microsoft.AspNetCore.Http;
using static System.Net.Mime.MediaTypeNames;
using System.Net;
using System.Text.Json;

namespace ChattingApp.Middleware
{
    public class ExceptopnMiddleware
    {
        public ExceptopnMiddleware(RequestDelegate next,ILogger<ExceptopnMiddleware> logger,IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public RequestDelegate _next { get; }
        public ILogger<ExceptopnMiddleware> _logger { get; }
        public IHostEnvironment _env { get; }

        public async Task IvokeAsync(HttpContext context)
        {

            try
            {
                await _next(context);
            }
            catch (Exception ex){

                _logger.LogError(ex, ex.Message);

                context.Response.ContentType = "application/json";

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                var response = _env.IsDevelopment()

                ? new ApiException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())

                : new ApiException(context.Response.StatusCode, ex.Message,"InternalServerError");
                var options= new JsonSerializerOptions { PropertyNamingPolicy=JsonNamingPolicy.CamelCase };

                var json = JsonSerializer.Serialize(response, options);

                await context.Response.WriteAsync(json);

            }

        }
    }
}
