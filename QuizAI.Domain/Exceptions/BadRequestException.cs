using System.Net;

namespace QuizAI.Domain.Exceptions;

public class BadRequestException : CustomException
{
    public BadRequestException(object details)
        : base("Bad Request", HttpStatusCode.BadRequest, details) { }
}
