using QuizAI.Domain.Entities;

namespace QuizAI.Domain.Repositories;

public interface IQuestionsRepository
{
    Task<Question?> GetByOrderAsync(Guid quizId, int order, bool answers = false);
}
