using QuizAI.Application.Common;
using QuizAI.Domain.Exceptions;
using Serilog;
using Serilog.Events;
using System.Net;
using System.Text.Json;

namespace QuizAI.API.Middlewares;

public class ErrorHandlingMiddleware : IMiddleware
{
    private const int MaxQueryLength = 1024;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next.Invoke(context);

            if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                throw new UnauthorizedException("User is not authenticated");
            }
        }
        catch (Exception ex)
        {
            var (title, statusCode, details, logLevel) = ex switch
            {
                CustomException customException => (customException.Title, customException.StatusCode, customException.Message, LogEventLevel.Error),
                _ => ("An Unexpected Error Occurred", HttpStatusCode.InternalServerError, ex.Message, LogEventLevel.Fatal)
            };
            
            LogError(context, details, logLevel, statusCode);
            await ReturnErrorResponse(context, title, statusCode, details);
        }
    }
    private async Task ReturnErrorResponse(HttpContext context, string title, HttpStatusCode statusCode, object details)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/problem+json";

        var errorResponse = new ErrorResponse(title, statusCode, details, context.Request.Path.Value!, context.TraceIdentifier);

        var responseJson = JsonSerializer.Serialize(errorResponse);
        await context.Response.WriteAsync(responseJson);
    }

    private void LogError(HttpContext context, object details, LogEventLevel logLevel, HttpStatusCode statusCode)
    {
        string query = context.Request.QueryString.ToString().TrimStart('?');

        var logResponse = new LogResponse(
            context.Request.Method,
            context.Request.Path,
            query.Length > MaxQueryLength ? query.Substring(0, MaxQueryLength) : query,
            context.TraceIdentifier,
            details,
            statusCode
            );

        Log.Write(logLevel, JsonSerializer.Serialize(logResponse));
    }
}
