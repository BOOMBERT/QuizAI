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
    
    public static void Validate(IFormFile file, int maxSizeInBytes)
    {
        if (file == null || file.Length == 0)
            throw new UnprocessableEntityException("The file is empty or missing");
        
        if (file.Length > maxSizeInBytes)
            throw new RequestEntityTooLargeException($"The file exceeds the maximum allowed size of {maxSizeInBytes / (1024 * 1024)} MB");

        var fileExtension = Path.GetExtension(file.FileName).ToLower();

        if (_supportedImageExtensions.Contains(fileExtension))
        {
            if (!ValidateImage(file))
                throw new UnprocessableEntityException("Invalid image file");
        }
        else
        {
            throw new UnsupportedMediaTypeException($"The file extension '{fileExtension}' is not supported");
        }
    }

    private static bool ValidateImage(IFormFile file)
    {
        if (!file.ContentType.StartsWith("image/"))
            return false;

        using (var stream = file.OpenReadStream())
        {
            var sizeForMagicNumber = _imageMagicNumbers.Max(mn => mn.Value.Length);
            var buffer = new byte[sizeForMagicNumber];

            stream.Read(buffer, 0, buffer.Length);

            foreach (var supportedImageExtension in _supportedImageExtensions)
            {
                if (IsMatchingMagicNumber(buffer, supportedImageExtension))
                    return true;
            }
            return false;
        }
    }

    private static bool IsMatchingMagicNumber(byte[] buffer, string fileExtension)
    {
        if (!_imageMagicNumbers.TryGetValue(fileExtension, out var fileMagicNumbers))
            throw new UnsupportedMediaTypeException($"The file extension '{fileExtension}' is not supported");

        return buffer.Length >= fileMagicNumbers.Length && buffer.Take(fileMagicNumbers.Length).SequenceEqual(fileMagicNumbers);
    }
}
