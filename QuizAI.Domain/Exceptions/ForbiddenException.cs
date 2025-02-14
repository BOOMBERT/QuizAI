using System.Net;

namespace QuizAI.Domain.Exceptions;

public class ForbiddenException : CustomException
{
    public ForbiddenException(object details)
    : base("Resource Forbidden", HttpStatusCode.Forbidden, details) { }
}
