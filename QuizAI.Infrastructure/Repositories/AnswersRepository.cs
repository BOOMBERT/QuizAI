using Microsoft.EntityFrameworkCore;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Repositories;
using QuizAI.Infrastructure.Persistence;

namespace QuizAI.Infrastructure.Repositories;

public class AnswersRepository : IAnswersRepository
{
    private readonly AppDbContext _context;

    public AnswersRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<string>> GetMultipleChoiceAnswersContentAsync(int questionId)
    {
        return await _context.MultipleChoiceAnswers
            .Where(mca => mca.QuestionId == questionId)
            .Select(mca => mca.Content)
            .ToArrayAsync();
    }

    public async Task<IEnumerable<UserAnswer>> GetUserAnswersByAttemptIdAsync(Guid quizAttemptId, bool includeQuestionsWithAnswers = false)
    {
        var baseQuery = _context.UserAnswers
            .AsNoTracking()
            .Where(ua => ua.QuizAttemptId == quizAttemptId);

        if (includeQuestionsWithAnswers)
        {
            baseQuery = baseQuery
                .Include(ua => ua.Question)
                    .ThenInclude(q => q.TrueFalseAnswer)
                .Include(ua => ua.Question)
                    .ThenInclude(q => q.MultipleChoiceAnswers)
                .Include(ua => ua.Question)
                    .ThenInclude(q => q.OpenEndedAnswer);
        }

        return await baseQuery.ToArrayAsync();
    }
}
