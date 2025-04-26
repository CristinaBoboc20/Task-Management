using Microsoft.EntityFrameworkCore.Storage.Json;
using Newtonsoft.Json;
using System.Net;
using TaskManagement.Helpers;

namespace TaskManagement.Middleware
{
    public class ExceptionMiddleware : IMiddleware
    {
        // Handle exceptions and forward to response
        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (KeyNotFoundException exception)
            {
                await HandleExceptionAsync(context, exception, HttpStatusCode.NotFound);
            }
            catch (UnauthorizedAccessException exception)
            {
                await HandleExceptionAsync(context, exception, HttpStatusCode.Unauthorized);
            }
            catch (Exception exception)
            {
                await HandleExceptionAsync(context, exception, HttpStatusCode.BadRequest);
            }
        }

        // Build and send error response
        private static async Task HandleExceptionAsync(HttpContext context, Exception exception, HttpStatusCode statusCode)
        {
            var response = context.Response;
            
            if (!response.HasStarted)
            {
                response.ContentType = "application/json";
                response.StatusCode = (int)statusCode;

                ApiResponse<string> errorResponse = new ApiResponse<string>((int)statusCode, exception.Message);

                await response.WriteAsync(JsonConvert.SerializeObject(errorResponse));
            }
        }

    }
}

