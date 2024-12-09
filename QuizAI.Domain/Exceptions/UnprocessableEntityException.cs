using System.Net;

namespace QuizAI.Domain.Exceptions;

public class UnprocessableEntityException : CustomException
{
    public UnprocessableEntityException(object details) 
        : base("Invalid Resource To Process", HttpStatusCode.UnprocessableEntity, details) { }
}
