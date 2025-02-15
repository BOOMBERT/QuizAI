using Microsoft.AspNetCore.Http;
using QuizAI.Domain.Entities;

namespace QuizAI.Application.Interfaces;

public interface IImageService
{
    Task<(byte[], string)> GetDataToReturnAsync(Quiz quiz, int? questionId = null);
    Task<Image> UploadAsync(IFormFile image);
    Task DeleteIfNotAssignedAsync(Guid imageId, Guid? quizIdToSkip = null, int? questionIdToSkip = null);
}
