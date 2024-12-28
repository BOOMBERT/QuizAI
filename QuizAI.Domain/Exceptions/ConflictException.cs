using System.Net;

namespace QuizAI.Domain.Exceptions;

public class ConflictException : CustomException
{
    public ConflictException(object details)
        : base("Conflict With Resource", HttpStatusCode.Conflict, details) { }
}
