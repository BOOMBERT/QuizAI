using Microsoft.AspNetCore.Http;
using QuizAI.Domain.Entities;

namespace QuizAI.Application.Interfaces;

public interface IImageService
{
    Task<(byte[], string)> GetDataToReturnAsync(Quiz quiz, int? questionId = null);
    Task<Image> UploadAsync(IFormFile image, bool isPrivate);
    Task DeleteIfNotAssignedAsync(Guid imageId, bool isPrivate, Guid? quizIdToSkip = null, int? questionIdToSkip = null);
    Task MoveImagesAsync(HashSet<(Guid imageId, string imageExtension)> images, bool isPrivate);
}
