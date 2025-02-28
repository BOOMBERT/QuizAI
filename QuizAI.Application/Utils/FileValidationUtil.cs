using Microsoft.AspNetCore.Http;
using QuizAI.Domain.Enums;
using QuizAI.Domain.Exceptions;

namespace QuizAI.Application.Utils;

public static class FileValidationUtil
{
    private static readonly Dictionary<string, byte[]> _imageMagicNumbers = new Dictionary<string, byte[]>
    {
        { FileExtension.Jpg,  new byte[] { 0xFF, 0xD8, 0xFF } },
        { FileExtension.Jpeg,  new byte[] { 0xFF, 0xD8, 0xFF } },
        { FileExtension.Png,  new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } }
    };

    private static readonly string[] _supportedImageExtensions = _imageMagicNumbers.Keys.ToArray();
    
    public static void Validate(IFormFile file, int imageMaxSizeInBytes)
    {
        if (file == null || file.Length == 0)
            throw new UnprocessableEntityException("The file is empty or missing");

        var fileExtension = Path.GetExtension(file.FileName).ToLower();

        if (_supportedImageExtensions.Contains(fileExtension))
        {
            if (file.Length > imageMaxSizeInBytes)
                throw new RequestEntityTooLargeException($"The image file exceeds the maximum allowed size of {imageMaxSizeInBytes / (1024 * 1024)} MB");

            if (!ValidateImage(file))
                throw new UnprocessableEntityException("Invalid image file");
        }
        else
        {
            throw new UnsupportedMediaTypeException($"The file extension {fileExtension} is not supported");
        }
    }

    private static bool ValidateImage(IFormFile file)
    {
        var imageExtension = Path.GetExtension(file.FileName).ToLower();
        var imageContentType = $"image/{(imageExtension == FileExtension.Jpg ? FileExtension.Jpeg.Substring(1) : imageExtension.Substring(1))}";

        if (!file.ContentType.Equals(imageContentType))
            return false;

        if (!_imageMagicNumbers.TryGetValue(imageExtension, out var imageMagicNumber))
            throw new UnsupportedMediaTypeException($"The file extension {imageExtension} is not supported");

        using var stream = file.OpenReadStream();
        var buffer = new byte[imageMagicNumber.Length];

        int bytesRead = stream.Read(buffer, 0, buffer.Length);
        if (bytesRead != imageMagicNumber.Length)
            return false;

        return buffer.SequenceEqual(imageMagicNumber);
    }
}
