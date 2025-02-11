using QuizAI.Domain.Entities;
using QuizAI.Domain.Enums;

namespace QuizAI.Domain.Repositories;

public interface IQuestionsRepository
{
    Task<IEnumerable<Question>> GetAllAsync(Guid quizId, bool answers = false);
    Task<Question?> GetByOrderAsync(Guid quizId, int order, bool answers = false);
    Task<Guid?> GetImageIdAsync(Guid quizId, int questionId);
}
