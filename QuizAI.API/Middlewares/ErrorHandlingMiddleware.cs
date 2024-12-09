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
        }
        catch (Exception ex)
        {
            var (title, statusCode, details, logLevel) = ex switch
            {
                CustomException customException => (customException.Title, customException.StatusCode, customException.Details, LogEventLevel.Error),
                _ => ("An Unexpected Error Occurred", HttpStatusCode.InternalServerError, ex.Message, LogEventLevel.Fatal)
            };

            LogError(context, details, logLevel, statusCode);
            await ReturnErrorResponse(context, title, statusCode, details);
        }
    }
    private async Task ReturnErrorResponse(HttpContext context, string title, HttpStatusCode statusCode, object details)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        var errorResponse = new ErrorResponse(title, statusCode, details, context.Request.Path.Value!, context.TraceIdentifier);

        var responseJson = JsonSerializer.Serialize(errorResponse);
        await context.Response.WriteAsync(responseJson);
    }

    private void LogError(HttpContext context, object details, LogEventLevel logLevel, HttpStatusCode statusCode)
    {
        string query = context.Request.QueryString.ToString().Substring(1);

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
