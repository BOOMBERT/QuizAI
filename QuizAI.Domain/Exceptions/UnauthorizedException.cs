using System.Net;

namespace QuizAI.Domain.Exceptions;

public class UnauthorizedException : CustomException
{
    public UnauthorizedException(object details)
    : base("Resource Not Authenticated", HttpStatusCode.Unauthorized, details) { }
}
