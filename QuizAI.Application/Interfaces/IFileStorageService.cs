namespace QuizAI.Application.Interfaces;

public interface IFileStorageService
{
    Task<Guid> UploadAsync(byte[] fileData, string fileExtension, bool isPrivate, Guid? filename = null);
    Task<byte[]> RetrieveAsync(Guid fileNameAsGuid, string fileExtension, bool isPrivate);
    void CopyImage(Guid filename, string fileExtension, bool isPrivate);
    void Delete(Guid filename, string fileExtension, bool isPrivate);
}
