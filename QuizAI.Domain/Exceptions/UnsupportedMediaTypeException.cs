using System.Net;

namespace QuizAI.Domain.Exceptions;

public class UnsupportedMediaTypeException : CustomException
{
    public UnsupportedMediaTypeException(object details) 
        : base("Unsupported Resource Type", HttpStatusCode.UnsupportedMediaType, details) { }
}
