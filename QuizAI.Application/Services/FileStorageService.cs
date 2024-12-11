using Microsoft.Extensions.Hosting;
using QuizAI.Application.Interfaces;
using QuizAI.Domain.Exceptions;

namespace QuizAI.Application.Services;

public class FileStorageService : IFileStorageService
{
    private readonly string _storagePath;

    public FileStorageService(IHostEnvironment environment, string folderName)
    {
        _storagePath = Path.Combine(environment.ContentRootPath, folderName);

        if (!Directory.Exists(_storagePath))
        {
            Directory.CreateDirectory(_storagePath);
        }
    }

    public async Task<Guid> UploadAsync(byte[] fileData, string fileExtension)
    {
        var fileNameAsGuid = Guid.NewGuid();
        var filePath = Path.Combine(_storagePath, fileNameAsGuid + fileExtension.ToLower());

        await File.WriteAllBytesAsync(filePath, fileData);

        return fileNameAsGuid;
    }

    public async Task<byte[]> RetrieveFileAsync(Guid fileName, string fileExtension)
    {
        var fullFilename = fileName.ToString() + fileExtension.ToLower();
        var filePath = Path.Combine(_storagePath, fullFilename);

        if (!File.Exists(filePath))
            throw new NotFoundException($"The file {fullFilename} could not be found.");

        return await File.ReadAllBytesAsync(filePath);
    }
}
