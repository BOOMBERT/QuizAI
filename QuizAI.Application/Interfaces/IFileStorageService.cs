namespace QuizAI.Application.Interfaces;

public interface IFileStorageService
{
    Task<Guid> UploadAsync(byte[] fileData, string fileExtension);
    Task<byte[]> RetrieveAsync(Guid fileNameAsGuid, string fileExtension);
    void Delete(Guid filename, string fileExtension);
}
