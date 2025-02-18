using QuizAI.Application.Interfaces;
using QuizAI.Domain.Enums;
using QuizAI.Domain.Exceptions;

namespace QuizAI.Application.Services;

public class FileStorageService : IFileStorageService
{
    private readonly string _publicStoragePath;
    private readonly string _privateStoragePath;

    public FileStorageService(string publicStoragePath, string privateStoragePath)
    {
        _publicStoragePath = publicStoragePath;
        _privateStoragePath = privateStoragePath;
    }

    public async Task<Guid> UploadAsync(byte[] fileData, string fileExtension, bool isPrivate, Guid? filename = null)
    {
        if (filename == null) filename = Guid.NewGuid();

        var fullFilename = filename.ToString() + fileExtension.ToLower();
        var filePath = Path.Combine(GetProperStoragePath(isPrivate), fullFilename);

        if (File.Exists(filePath))
            throw new ConflictException($"The file {fullFilename} already exists");

        await File.WriteAllBytesAsync(filePath, fileData);

        return (Guid)filename;
    }

    public async Task<byte[]> RetrieveAsync(Guid filename, string fileExtension, bool isPrivate)
    {
        var filePath = GetFullFilePathIfExists(filename, fileExtension, isPrivate);

        return await File.ReadAllBytesAsync(filePath);
    }

    public void Delete(Guid filename, string fileExtension, bool isPrivate)
    {
        var filePath = GetFullFilePathIfExists(filename, fileExtension, isPrivate);

        File.Delete(filePath);
    }

    public void CopyImage(Guid filename, string fileExtension, bool isPrivate)
    {
        string oldFullFilePath = GetFullFilePathIfExists(filename, fileExtension, isPrivate);
        string newFullFilePath = Path.Combine(GetProperStoragePath(!isPrivate), filename.ToString() + fileExtension.ToLower());

        File.Copy(oldFullFilePath, newFullFilePath, overwrite: false);
    }

    private string GetFullFilePathIfExists(Guid filename, string fileExtension, bool isPrivate)
    {
        var fullFilename = filename.ToString() + fileExtension.ToLower();
        var filePath = Path.Combine(GetProperStoragePath(isPrivate), fullFilename);

        if (!File.Exists(filePath))
            throw new NotFoundException($"The file {fullFilename} could not be found");

        return filePath;
    }

    private string GetProperStoragePath(bool isPrivate) => 
        isPrivate ? _privateStoragePath : _publicStoragePath;
}
