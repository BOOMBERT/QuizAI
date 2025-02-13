using QuizAI.Domain.Entities;

namespace QuizAI.Domain.Repositories;

public interface IQuizPermissionsRepository
{
    Task<QuizPermission?> GetAsync(Guid quizId, string userId);
    Task<IEnumerable<QuizPermission>> GetAllAsync(Guid quizId);
    Task UpdateQuizIdAsync(Guid previousQuizId, Guid newQuizId);
}
