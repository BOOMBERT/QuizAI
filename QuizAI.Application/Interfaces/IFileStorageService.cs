using Microsoft.AspNetCore.Http;

namespace QuizAI.Application.Interfaces;

public interface IFileStorageService
{
    Task<Guid> UploadFileAsync(IFormFile file);
}
