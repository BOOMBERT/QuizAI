using QuizAI.Domain.Entities;
using QuizAI.Domain.Enums;

namespace QuizAI.Domain.Repositories;

public interface IQuestionsRepository
{
    Task<Question?> GetWithAnswerAsync(Guid quizId, int questionId, QuestionType questionType);
}
