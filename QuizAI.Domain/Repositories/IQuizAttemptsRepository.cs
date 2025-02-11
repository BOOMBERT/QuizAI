using QuizAI.Domain.Entities;

namespace QuizAI.Domain.Repositories;

public interface IQuizAttemptsRepository
{
    Task<QuizAttempt?> GetUnfinishedAsync(Guid quizId, string userId);
    Task<QuizAttempt?> GetFinishedByIdAsync(Guid quizId, Guid quizAttemptId, string userId);
    Task<QuizAttempt?> GetLatestFinishedAsync(Guid quizId, string userId);
    Task<bool> HasAnyAsync(Guid quizId);
}
