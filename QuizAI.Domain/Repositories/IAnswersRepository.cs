using QuizAI.Domain.Entities;

namespace QuizAI.Domain.Repositories;

public interface IAnswersRepository
{
    Task<IEnumerable<string>> GetMultipleChoiceAnswersContentAsync(int questionId);
    Task<IEnumerable<UserAnswer>> GetUserAnswersByAttemptIdAsync(Guid quizAttemptId, bool includeQuestionsWithAnswers = false);
}
