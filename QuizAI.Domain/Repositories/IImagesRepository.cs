using QuizAI.Domain.Entities;

namespace QuizAI.Domain.Repositories;

public interface IImagesRepository
{
    Task<Image?> GetAsync(byte[] hash);
    Task<string?> GetExtensionAsync(Guid id);
    Task<bool> IsAssignedToAnyQuizAsync(Guid imageId);
}
