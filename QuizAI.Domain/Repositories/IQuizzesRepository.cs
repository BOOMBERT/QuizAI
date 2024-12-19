using QuizAI.Domain.Entities;

namespace QuizAI.Domain.Repositories;

public interface IQuizzesRepository
{
    Task<Quiz?> GetWithCategoriesAsync(Guid quizId);
    Task<Guid?> GetImageId(Guid quizId);
}
