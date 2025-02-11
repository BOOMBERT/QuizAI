using Microsoft.EntityFrameworkCore;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Enums;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;
using QuizAI.Infrastructure.Persistence;
using System.Linq.Expressions;

namespace QuizAI.Infrastructure.Repositories;

public class QuestionsRepository : IQuestionsRepository
{
    private readonly AppDbContext _context;

    public QuestionsRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Question>> GetAllAsync(Guid quizId, bool answers = false)
    {
        var baseQuery = _context.Questions
            .AsNoTracking()
            .Where(qn => qn.QuizId == quizId);

        if (answers)
        {
            baseQuery = baseQuery
                .Include(qn => qn.MultipleChoiceAnswers)
                .Include(qn => qn.OpenEndedAnswer)
                .Include(qn => qn.TrueFalseAnswer);
        }

        return await baseQuery.ToListAsync();
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

    public async Task<Guid?> GetImageIdAsync(Guid quizId, int questionId)
    {
        var question = await _context.Questions
            .Where(qn => qn.QuizId == quizId && qn.Id == questionId)
            .FirstOrDefaultAsync() 
            ?? throw new NotFoundException($"Question with ID {questionId} in quiz with ID {quizId} was not found.");

        return question.ImageId;
    }
}
