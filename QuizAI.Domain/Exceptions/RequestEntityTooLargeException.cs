using System.Net;

namespace QuizAI.Domain.Exceptions;

public class RequestEntityTooLargeException : CustomException
{
    public RequestEntityTooLargeException(object details) 
        : base("Resource Too Large", HttpStatusCode.RequestEntityTooLarge, details) { }
}
