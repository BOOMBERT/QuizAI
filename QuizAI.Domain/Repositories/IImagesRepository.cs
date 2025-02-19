using QuizAI.Domain.Entities;

namespace QuizAI.Domain.Repositories;

public interface IImagesRepository
{
    Task<Image?> GetAsync(byte[] hash);
    Task<bool> IsAssignedToAnyQuizAsync(Guid imageId, Guid? quizIdToSkip, bool? onlyPrivate = null);
    Task<bool> IsAssignedToAnyQuestionAsync(Guid imageId, int? questionIdToSkip, bool? onlyPrivate = null);
    Task<IEnumerable<Image>> GetQuizAndItsQuestionImagesAsync(Guid quizId, Guid? quizImageId);
    Task<string?> GetFileExtensionAsync(Guid imageId);
}
