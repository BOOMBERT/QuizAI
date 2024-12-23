using Microsoft.AspNetCore.Http;
using QuizAI.Domain.Entities;

namespace QuizAI.Application.Interfaces;

public interface IImageService
{
    Task<Image> UploadAsync(IFormFile image);
    Task DeleteIfNotAssignedAsync(Guid imageId);
}
