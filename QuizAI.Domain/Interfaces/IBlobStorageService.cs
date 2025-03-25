namespace QuizAI.Domain.Interfaces;

public interface IBlobStorageService
{
    Task<Guid> UploadAsync(Stream fileStream, string fileExtension, bool isPrivate, Guid? filename = null);
    Task<byte[]> RetrieveAsync(Guid fileNameAsGuid, string fileExtension, bool isPrivate);
    Task DeleteAsync(Guid filename, string fileExtension, bool isPrivate);
    Task CopyImageAsync(Guid filename, string fileExtension, bool isPrivate);
}
