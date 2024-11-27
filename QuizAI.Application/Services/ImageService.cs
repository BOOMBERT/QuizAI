using Microsoft.AspNetCore.Http;
using QuizAI.Application.Interfaces;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Repositories;
using Shipwreck.Phash;
using Shipwreck.Phash.Bitmaps;

namespace QuizAI.Application.Services;

public class ImageService : IImageService
{
    private readonly IImagesRepository _imagesRepository;
    private readonly IFileStorageService _fileStorageService;

    public ImageService(IImagesRepository imagesRepository, IFileStorageService fileStorageService)
    {
        _imagesRepository = imagesRepository;
        _fileStorageService = fileStorageService;
    }

    public async Task<Image> UploadAsync(IFormFile image)
    {
        var imageHash = HashImageByPhash(image);

        var imageInDb = await _imagesRepository.GetAsync(imageHash);

        if (imageInDb != null)
        {
            return imageInDb;
        }

        var imageName = await _fileStorageService.UploadFileAsync(image);

        return new Image
        {
            Id = imageName,
            FileExtension = Path.GetExtension(image.FileName),
            Hash = imageHash
        };
    }

    private byte[] HashImageByPhash(IFormFile image)
    {
        using (var memoryStream = new MemoryStream())
        {
            image.CopyTo(memoryStream);
            byte[] imageBytes = memoryStream.ToArray();

            using (var bitmap = new System.Drawing.Bitmap(new MemoryStream(imageBytes)))
            {
                var luminanceImage = bitmap.ToLuminanceImage();

                var hash = ImagePhash.ComputeDigest(luminanceImage);
                return Convert.FromHexString(hash.ToString().AsSpan(2));
            }
        }
    }
}
