using QuizAI.Domain.Entities;
using QuizAI.Domain.Enums;

namespace QuizAI.Domain.Repositories;

public interface IQuestionsRepository
{
    Task<Question?> GetWithAnswerAsync(Guid quizId, int questionId, QuestionType questionType);
    Task<Guid?> GetImageIdAsync(Guid quizId, int questionId);
    Task<Question?> GetByOrderAsync(Guid quizId, int order);
    Task UpdateImageAsync(Guid quizId, int questionId, Guid? imageId);
    Task<IEnumerable<string>> GetMultipleChoiceAnswersContentAsync(int questionId);
}
