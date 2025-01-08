using QuizAI.Domain.Entities;
using QuizAI.Domain.Enums;

namespace QuizAI.Domain.Repositories;

public interface IQuestionsRepository
{
    Task<ICollection<Question>> GetAsync(Guid quizId, bool answers = false);
    Task<Question?> GetWithAnswerAsync(Guid quizId, int questionId, QuestionType questionType);
    Task<Question?> GetByOrderAsync(Guid quizId, int order);
    Task<IEnumerable<string>> GetMultipleChoiceAnswersContentAsync(int questionId);
    Task<Guid?> GetImageIdAsync(Guid quizId, int questionId);
    Task<IEnumerable<Guid>> GetImagesNamesAsync(Guid quizId);
    Task UpdateImageAsync(int questionId, Guid? imageId);
    Task<int> HowManyAsync(Guid quizId);

}
