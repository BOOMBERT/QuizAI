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

    public async Task<Question?> GetMultipleChoiceWithAnswersAsync(Guid quizId, int questionId)
    {
        return await _context.Questions
            .Include(qn => qn.MultipleChoiceAnswers)
            .FirstOrDefaultAsync(qn => qn.Id == questionId && qn.QuizId == quizId);
    }
}
