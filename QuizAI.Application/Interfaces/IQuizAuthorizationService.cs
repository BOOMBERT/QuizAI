using QuizAI.Domain.Constants;
using QuizAI.Domain.Entities;

namespace QuizAI.Application.Interfaces;

public interface IQuizAuthorizationService
{
    Task AuthorizeAsync(Quiz quiz, string? userId, ResourceOperation resourceOperation);
    Task<bool> AuthorizeReadOperationAndGetCanEditAsync(Quiz quiz, string? userId = null);
}
