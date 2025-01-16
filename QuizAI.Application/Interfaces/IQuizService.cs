using QuizAI.Domain.Entities;

namespace QuizAI.Application.Interfaces;

public interface IQuizService
{
    Task DeprecateAsync(Quiz oldQuiz, Guid newQuizId);
}
