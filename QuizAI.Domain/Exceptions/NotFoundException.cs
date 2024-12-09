using System.Net;

namespace QuizAI.Domain.Exceptions;

public class NotFoundException : CustomException
{
    public NotFoundException(object details)
        : base("Resource Not Found", HttpStatusCode.NotFound, details) { }
}
