using QuizAI.Domain.Entities;

namespace QuizAI.Domain.Repositories;

public interface IImagesRepository
{
    Task<Image?> GetAsync(byte[] hash);
}
