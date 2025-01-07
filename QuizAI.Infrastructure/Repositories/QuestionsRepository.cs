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
}
