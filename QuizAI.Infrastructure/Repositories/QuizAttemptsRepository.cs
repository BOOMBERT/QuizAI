using Microsoft.EntityFrameworkCore;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Repositories;
using QuizAI.Infrastructure.Persistence;

namespace QuizAI.Infrastructure.Repositories;

public class QuizAttemptsRepository : IQuizAttemptsRepository
{
    private readonly AppDbContext _context;

    public QuizAttemptsRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<QuizAttempt?> GetUnfinishedAsync(Guid quizId, string userId)
    {
        return await _context.QuizAttempts
            .Where(qa => qa.QuizId == quizId && qa.UserId == userId && qa.FinishedAt == null)
            .FirstOrDefaultAsync();
    }

    public async Task<QuizAttempt?> GetFinishedByIdAsync(Guid quizId, Guid quizAttemptId, string userId)
    {
        return await _context.QuizAttempts
            .AsNoTracking()
            .Where(qa => qa.Id == quizAttemptId && qa.QuizId == quizId && qa.UserId == userId && qa.FinishedAt != null)
            .FirstOrDefaultAsync();
    }

    public async Task<QuizAttempt?> GetLatestFinishedAsync(Guid quizId, string userId)
    {
        return await _context.QuizAttempts
            .AsNoTracking()
            .Where(qa => qa.QuizId == quizId && qa.UserId == userId && qa.FinishedAt != null)
            .OrderByDescending(qa => qa.FinishedAt)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> HasAnyAsync(Guid quizId)
    {
        return await _context.QuizAttempts
            .AnyAsync(qa => qa.QuizId == quizId);
    }
}
