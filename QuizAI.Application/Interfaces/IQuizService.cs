using QuizAI.Domain.Entities;

namespace QuizAI.Application.Interfaces;

public interface IQuizService
{
    Task<(Quiz, bool)> GetValidOrDeprecateAndCreateWithNewQuestionsAsync(Guid oldQuizId);
    Task<(Quiz, bool)> GetValidOrDeprecateAndCreateWithQuestionsAsync(Guid quizId, int? questionId = null);
}
