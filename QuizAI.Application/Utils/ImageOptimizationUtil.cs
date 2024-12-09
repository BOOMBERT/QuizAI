using Microsoft.AspNetCore.Http;
using QuizAI.Domain.Enums;
using QuizAI.Domain.Exceptions;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace QuizAI.Application.Utils;

public static class ImageOptimizationUtil
{
    public static async Task<byte[]> Optimize(
        IFormFile imageToOptimize, (ushort width, ushort height)? newSize, CompressionSetting compressionSetting = CompressionSetting.Medium)
    {
        var imageExtension = Path.GetExtension(imageToOptimize.FileName);

        using (var image = await SixLabors.ImageSharp.Image.LoadAsync(imageToOptimize.OpenReadStream()))
        {
            if (newSize != null)
                Resize(image, ((ushort width, ushort height))newSize);

            return await Compress(image, imageExtension, compressionSetting);
        }
    }

    private static void Resize(SixLabors.ImageSharp.Image image, (ushort width, ushort height) newSize)
    {
        image.Mutate(x => x.Resize(new ResizeOptions
        {
            Mode = ResizeMode.Max,
            Size = new SixLabors.ImageSharp.Size(newSize.width, newSize.height)
        }));
    }

    private static async Task<byte[]> Compress(SixLabors.ImageSharp.Image image, string fileExtension, CompressionSetting compressionSetting)
    {
        fileExtension = fileExtension.ToLower();

        using (var memoryStream = new MemoryStream())
        {
            if (fileExtension == FileExtension.Jpg || fileExtension == FileExtension.Jpeg)
            {
                var jpegEncoder = new JpegEncoder
                {
                    Quality = (byte)compressionSetting
                };
                await image.SaveAsync(memoryStream, jpegEncoder);
            }
            else if (fileExtension == FileExtension.Png)
            {
                var pngEncoder = new PngEncoder
                {
                    CompressionLevel = GetPngCompressionLevel(compressionSetting)
                };
                await image.SaveAsync(memoryStream, pngEncoder);
            }
            else
            {
                throw new UnsupportedMediaTypeException($"The file extension '{fileExtension}' is not supported");
            }

            return memoryStream.ToArray();
        }
    }

    private static PngCompressionLevel GetPngCompressionLevel(CompressionSetting compressionSetting)
    {
        return (compressionSetting) switch
        {
            CompressionSetting.None => PngCompressionLevel.Level0,
            CompressionSetting.Low => PngCompressionLevel.Level9,
            CompressionSetting.Medium => PngCompressionLevel.Level8,
            CompressionSetting.High => PngCompressionLevel.Level7,
            CompressionSetting.Maximum => PngCompressionLevel.Level6,
            _ => throw new UnprocessableEntityException($"The compression setting '{compressionSetting}' is not supported")
        };
    }
}
