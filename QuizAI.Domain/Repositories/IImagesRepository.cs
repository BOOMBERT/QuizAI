using QuizAI.Domain.Entities;

namespace QuizAI.Domain.Repositories;

public interface IImagesRepository
{
    Task<Image?> GetAsync(byte[] hash);
    Task<bool> IsAssignedToAnyQuizAsync(Guid imageId, Guid? quizIdToSkip);
    Task<bool> IsAssignedToAnyQuestionAsync(Guid imageId, int? questionIdToSkip);
}
