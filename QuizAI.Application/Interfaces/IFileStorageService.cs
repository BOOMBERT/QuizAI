using Microsoft.AspNetCore.Http;

namespace QuizAI.Application.Interfaces;

public interface IFileStorageService
{
    Task<Guid> UploadAsync(byte[] fileData, string fileExtension);
    Task<byte[]> RetrieveAsync(Guid fileNameAsGuid, string fileExtension);
}
