using Azure.Storage.Blobs;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Interfaces;

namespace QuizAI.Infrastructure.Storage;

public class BlobStorageService : IBlobStorageService
{
    private readonly BlobServiceClient _serviceClient;
    private readonly string _publicContainerName;
    private readonly string _privateContainerName;

    public BlobStorageService(string connectionString, string publicContainerName, string privateContainerName)
    {
        _serviceClient = new BlobServiceClient(connectionString);
        _publicContainerName = publicContainerName;
        _privateContainerName = privateContainerName;
    }

    private BlobContainerClient GetContainerClient(bool privateStorage)
    {
        return _serviceClient.GetBlobContainerClient(privateStorage ? _privateContainerName : _publicContainerName);
    }

    private BlobClient GetBlobClient(Guid filename, string fileExtension, bool isPrivate)
    {
        var fullFilename = filename.ToString() + fileExtension.ToLower();
        var containerClient = GetContainerClient(isPrivate);

        return containerClient.GetBlobClient(fullFilename);
    }

    public async Task<Guid> UploadAsync(Stream fileStream, string fileExtension, bool isPrivate, Guid? filename = null)
    {
        filename ??= Guid.NewGuid();
        var blobClient = GetBlobClient(filename.Value, fileExtension, isPrivate);

        if (await blobClient.ExistsAsync())
            throw new ConflictException($"The file {filename}{fileExtension} already exists");

        await blobClient.UploadAsync(fileStream);

        return filename.Value;
    }

    public async Task<byte[]> RetrieveAsync(Guid filename, string fileExtension, bool isPrivate)
    {
        var blobClient = GetBlobClient(filename, fileExtension, isPrivate);

        if (!await blobClient.ExistsAsync())
            throw new NotFoundException($"The file {filename}{fileExtension} could not be found");

        var downloadInfo = await blobClient.DownloadAsync();
        using var memoryStream = new MemoryStream();
        await downloadInfo.Value.Content.CopyToAsync(memoryStream);

        return memoryStream.ToArray();
    }

    public async Task DeleteAsync(Guid filename, string fileExtension, bool isPrivate)
    {
        var blobClient = GetBlobClient(filename, fileExtension, isPrivate);

        var response = await blobClient.DeleteIfExistsAsync();

        if (!response.Value)
            throw new NotFoundException($"The file {filename}{fileExtension} could not be found");
    }

    public async Task CopyImageAsync(Guid filename, string fileExtension, bool isPrivate)
    {
        var sourceBlobClient = GetBlobClient(filename, fileExtension, isPrivate);
        var destinationBlobClient = GetBlobClient(filename, fileExtension, !isPrivate);

        if (!await sourceBlobClient.ExistsAsync())
            throw new NotFoundException($"The source file {filename}{fileExtension} could not be found");

        if (!await destinationBlobClient.ExistsAsync())
            await destinationBlobClient.StartCopyFromUriAsync(sourceBlobClient.Uri);
    }
}
