using System.Net;

namespace QuizAI.Domain.Exceptions;

public class CustomException : Exception
{
    public string Title { get; init; }
    public HttpStatusCode StatusCode { get; init; }
    public object Details { get; init; }

    public CustomException(string title, HttpStatusCode statusCode, object details)
    {
        Title = title;
        StatusCode = statusCode;
        Details = details;
    }
}
