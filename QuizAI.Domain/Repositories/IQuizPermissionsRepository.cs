using QuizAI.Domain.Entities;

namespace QuizAI.Domain.Repositories;

public interface IQuizPermissionsRepository
{
    Task<QuizPermission?> GetAsync(Guid quizId, string userId, bool trackChanges = true);
    Task<IEnumerable<QuizPermission>> GetAllAsync(Guid quizId);
    Task DeletePermissionsAsync(Guid quizId);
    Task<bool> CheckAsync(Guid quizId, string userId, bool? canEdit, bool? canPlay);
}
