using QuizAI.Domain.Entities;
using QuizAI.Domain.Enums;

namespace QuizAI.Domain.Repositories;

public interface IQuestionsRepository
{
    Task<Question?> GetByOrderAsync(Guid quizId, int order, bool answers = false);
    Task<Guid?> GetImageIdAsync(Guid quizId, int questionId);
}
