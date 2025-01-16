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

    public async Task<ICollection<Question>> GetAllAsync(Guid quizId, bool answers = false)
    {
        var baseQuery = _context.Questions
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

    public async Task<Question?> GetWithAnswerAsync(Guid quizId, int questionId, QuestionType questionType)
    {
        var baseQuery = _context.Questions.AsQueryable();

        baseQuery = questionType switch
        {
            QuestionType.MultipleChoice => baseQuery.Include(qn => qn.MultipleChoiceAnswers),
            QuestionType.OpenEnded => baseQuery.Include(qn => qn.OpenEndedAnswer),
            QuestionType.TrueFalse => baseQuery.Include(qn => qn.TrueFalseAnswer),
            _ => throw new UnsupportedMediaTypeException(
                $"Unsupported question type {questionType} for quiz with ID {quizId} and question with ID {questionId}"),
        };

        return await baseQuery
            .FirstOrDefaultAsync(qn => qn.Id == questionId && qn.QuizId == quizId && qn.Type == questionType);
    }

    public async Task<Question?> GetByOrderAsync(Guid quizId, int order)
    {
        return await _context.Questions
            .AsNoTracking()
            .Where(qn => qn.QuizId == quizId && qn.Order == order)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<string>> GetMultipleChoiceAnswersContentAsync(int questionId)
    {
        return await _context.MultipleChoiceAnswers
            .Where(mca => mca.QuestionId == questionId)
            .Select(mca => mca.Content)
            .ToArrayAsync();
    }

    public async Task<Guid?> GetImageIdAsync(Guid quizId, int questionId)
    {
        var question = await _context.Questions
            .Where(qn => qn.QuizId == quizId && qn.Id == questionId)
            .FirstOrDefaultAsync() 
            ?? throw new NotFoundException($"Question with ID {questionId} in quiz with ID {quizId} was not found.");

        return question.ImageId;
    }

    public async Task<IEnumerable<Guid>> GetImagesNamesAsync(Guid quizId)
    {
        return await _context.Questions
            .Where(qn => qn.QuizId == quizId && qn.ImageId != null)
            .Select(qn => (Guid)qn.ImageId!)
            .ToArrayAsync();
    }

    public async Task UpdateImageAsync(int questionId, Guid? imageId)
    {
        await _context.Questions
            .Where(qn => qn.Id == questionId)
            .ExecuteUpdateAsync(qn => qn.SetProperty(x => x.ImageId, imageId));
    }

    public async Task<int> HowManyAsync(Guid quizId)
    {
        return await _context.Questions
            .CountAsync(qn => qn.QuizId == quizId);
    }
}
