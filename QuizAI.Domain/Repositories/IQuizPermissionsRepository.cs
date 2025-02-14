using QuizAI.Domain.Entities;

namespace QuizAI.Domain.Repositories;

public interface IQuizPermissionsRepository
{
    Task<QuizPermission?> GetAsync(Guid quizId, string userId);
    Task<IEnumerable<QuizPermission>> GetAllAsync(Guid quizId);
    Task UpdateQuizIdAsync(Guid previousQuizId, Guid newQuizId);
    Task DeletePermissionsAsync(Guid quizId);
    Task<bool> HasAnyAsync(Guid quizId, string userId);
    Task<bool> CanEditAsync(Guid quizId, string userId);
}
