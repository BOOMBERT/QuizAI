using Microsoft.EntityFrameworkCore;
using QuizAI.Domain.Constants;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Repositories;
using QuizAI.Infrastructure.Persistence;
using System.Linq.Expressions;

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

    public async Task<QuizAttempt?> GetFinishedByIdAsync(Guid quizAttemptId, string userId)
    {
        return await _context.QuizAttempts
            .AsNoTracking()
            .Where(qa => qa.Id == quizAttemptId && qa.UserId == userId && qa.FinishedAt != null)
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

    public async Task<(IEnumerable<QuizAttempt>, int)> GetAllMatchingFinishedAsync(
        string userId,
        string? searchPhrase,
        int pageSize,
        int pageNumber,
        string? sortBy,
        SortDirection? sortDirection,
        Guid? filterByQuizId,
        DateTime? filterByStartedAtYearAndMonth,
        DateTime? filterByFinishedAtYearAndMonth)
    {
        var baseQuery = _context.QuizAttempts
            .AsNoTracking()
            .Include(qa => qa.Quiz)
            .Where(qa => qa.UserId == userId && qa.FinishedAt != null);

        if (filterByQuizId != null)
            baseQuery = baseQuery
                .Where(qa => qa.QuizId == filterByQuizId);

        if (filterByStartedAtYearAndMonth != null)
        {
            baseQuery = baseQuery
                .Where(qa => qa.StartedAt.Year == filterByStartedAtYearAndMonth.Value.Year && 
                qa.StartedAt.Month == filterByStartedAtYearAndMonth.Value.Month);
        }

        if (filterByFinishedAtYearAndMonth != null)
        {
            baseQuery = baseQuery
                .Where(qa => qa.FinishedAt!.Value.Year == filterByFinishedAtYearAndMonth.Value.Year && 
                qa.FinishedAt!.Value.Month == filterByFinishedAtYearAndMonth.Value.Month);
        }

        if (!string.IsNullOrEmpty(searchPhrase))
        {
            var searchPhraseLower = searchPhrase.ToLower();

            baseQuery = baseQuery.Where(qa => qa.Quiz.Name.ToLower().Contains(searchPhraseLower));
        }

        var totalCount = await baseQuery.CountAsync();

        if (!string.IsNullOrEmpty(sortBy) && sortDirection != null)
        {
            var columnsSelector = new Dictionary<string, Expression<Func<QuizAttempt, object>>>(StringComparer.OrdinalIgnoreCase)
            {
                { nameof(QuizAttempt.StartedAt), qa => qa.StartedAt },
                { nameof(QuizAttempt.FinishedAt), qa => qa.FinishedAt! },
            };

            if (columnsSelector.ContainsKey(sortBy))
            {
                var selectedColumn = columnsSelector[sortBy];

                baseQuery = sortDirection == SortDirection.Ascending
                    ? baseQuery.OrderBy(selectedColumn)
                    : baseQuery.OrderByDescending(selectedColumn);
            }
        }

        var quizAttempts = await baseQuery
            .Skip(pageSize * (pageNumber - 1))
            .Take(pageSize)
            .ToListAsync();

        return (quizAttempts, totalCount);
    }

    public async Task<bool> HasAnyAsync(Guid quizId)
    {
        return await _context.QuizAttempts
            .AnyAsync(qa => qa.QuizId == quizId);
    }
}
