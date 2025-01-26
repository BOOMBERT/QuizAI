using QuizAI.Domain.Entities;

namespace QuizAI.Application.Interfaces;

public interface IQuizService
{
    Task<Quiz> GetNewAndDeprecateOldAsync(Guid oldQuizId);
    Task<Quiz> GetNewWithCopiedQuestionsAndDeprecateOldAsync(Guid quizId, int questionId);
}
