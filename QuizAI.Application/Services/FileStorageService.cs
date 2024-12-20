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
        var filenameAsGuid = Guid.NewGuid();
        var filePath = Path.Combine(_storagePath, filenameAsGuid + fileExtension.ToLower());

        await File.WriteAllBytesAsync(filePath, fileData);

        return filenameAsGuid;
    }

    public async Task<byte[]> RetrieveAsync(Guid filename, string fileExtension)
    {
        var filePath = GetFullFilePathIfExists(filename, fileExtension);

        return await File.ReadAllBytesAsync(filePath);
    }

    public void Delete(Guid filename, string fileExtension)
    {
        var filePath = GetFullFilePathIfExists(filename, fileExtension);

        File.Delete(filePath);
    }

    private string GetFullFilePathIfExists(Guid filename, string fileExtension)
    {
        var fullFilename = filename.ToString() + fileExtension.ToLower();
        var filePath = Path.Combine(_storagePath, fullFilename);

        if (!File.Exists(filePath))
            throw new NotFoundException($"The file {fullFilename} could not be found.");

        return filePath;
    }
}
