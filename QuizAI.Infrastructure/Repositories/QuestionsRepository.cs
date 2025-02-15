using Microsoft.EntityFrameworkCore;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Repositories;
using QuizAI.Infrastructure.Persistence;

namespace QuizAI.Infrastructure.Repositories;

public class QuestionsRepository : IQuestionsRepository
{
    private readonly AppDbContext _context;

    public QuestionsRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Question?> GetByOrderAsync(Guid quizId, int order, bool answers = false)
    {
        var baseQuery = _context.Questions
            .AsNoTracking()
            .Where(qn => qn.QuizId == quizId && qn.Order == order);

        if (answers)
        {
            baseQuery = baseQuery
                .Include(qn => qn.MultipleChoiceAnswers)
                .Include(qn => qn.OpenEndedAnswer)
                .Include(qn => qn.TrueFalseAnswer);
        }

        return await baseQuery.FirstOrDefaultAsync();
    }
}
