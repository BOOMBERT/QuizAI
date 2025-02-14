using QuizAI.Domain.Constants;
using QuizAI.Domain.Entities;

namespace QuizAI.Application.Interfaces;

public interface IQuizAuthorizationService
{
    Task<bool> AuthorizeAsync(Quiz quiz, string? userId, ResourceOperation resourceOperation);
}
