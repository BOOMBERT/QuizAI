using Microsoft.AspNetCore.Http;
using QuizAI.Domain.Entities;

namespace QuizAI.Application.Interfaces;

public interface IImageService
{
    Task<(byte[], string)> GetDataToReturnAsync(Guid quizId, int? questionId = null);
    Task UpdateAsync(IFormFile image, Guid quizId, int? questionId = null);
    Task DeleteAsync(Guid quizId, int? questionId = null);
    Task<Image> UploadAsync(IFormFile image);
    Task DeleteIfNotAssignedAsync(Guid imageId);
}
