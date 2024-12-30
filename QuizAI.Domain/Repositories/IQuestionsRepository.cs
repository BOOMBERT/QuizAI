using QuizAI.Domain.Entities;

namespace QuizAI.Domain.Repositories;

public interface IQuestionsRepository
{
    Task<Question?> GetMultipleChoiceWithAnswersAsync(Guid quizId, int questionId);
}
