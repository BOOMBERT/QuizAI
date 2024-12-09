using QuizAI.Application.Common;
using QuizAI.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace QuizAI.API.Middlewares;

public class ErrorHandlingMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next.Invoke(context);
        }
        catch (Exception ex)
        {
            var (title, statusCode, details) = ex switch
            {
                CustomException customException => (customException.Title, customException.StatusCode, customException.Details),
                _ => ("An Unexpected Error Occurred", HttpStatusCode.InternalServerError, ex.Message)
            };

            await RespondToError(context, title, statusCode, details);
        }
    }
    private async Task RespondToError(HttpContext context, string title, HttpStatusCode statusCode, object details)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var errorResponse = new ErrorResponse(title, statusCode, details, context.Request.Path.Value!);

        var responseJson = JsonSerializer.Serialize(errorResponse);
        await context.Response.WriteAsync(responseJson);
    }
}
