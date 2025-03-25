using Microsoft.AspNetCore.Http;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Utils;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Enums;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Interfaces;
using QuizAI.Domain.Repositories;
using Shipwreck.Phash;
using SixLabors.ImageSharp.PixelFormats;
using System.IO;

namespace QuizAI.Application.Services;

public class ImageService : IImageService
{
    private readonly IRepository _repository;
    private readonly IBlobStorageService _blobStorageService;
    private readonly IImagesRepository _imagesRepository;
    private readonly (ushort width, ushort height)? _imagesDefaultSize;

    public ImageService(IRepository repository, IBlobStorageService fileStorageService, IImagesRepository imagesRepository, (ushort width, ushort height)? imagesDefaultSize)
    {
        _repository = repository;
        _blobStorageService = fileStorageService;
        _imagesRepository = imagesRepository;
        _imagesDefaultSize = imagesDefaultSize;
    }

    public async Task<(byte[], string)> GetDataToReturnAsync(Quiz quiz, int? questionId = null)
    {
        Guid imageName;
        string questionErrorMessageContext = string.Empty;

        if (questionId == null)
        {
            imageName = quiz.ImageId ?? throw new NotFoundException($"Quiz with ID {quiz.Id} has no associated image");
        }
        else
        {
            var question = await _repository.GetEntityAsync<Question>((int)questionId)
                ?? throw new NotFoundException($"Question with ID {questionId} in quiz with ID {quiz.Id} was not found");

            imageName = question.ImageId ?? throw new NotFoundException($"Question with ID {questionId} in quiz with ID {quiz.Id} has no associated image");
            questionErrorMessageContext = $"question with ID {questionId} in ";
        }

        var imageExtension = await _repository.GetFieldAsync<Image, string>(imageName, "FileExtension")
            ?? throw new NotFoundException($"Image for {questionErrorMessageContext}quiz with ID {quiz.Id} could not be found");

        var imageData = await _blobStorageService.RetrieveAsync(imageName, imageExtension, quiz.IsPrivate);

        if (imageExtension == FileExtension.Jpg) imageExtension = FileExtension.Jpeg;
        var contentType = "image/" + imageExtension.TrimStart('.');
        return (imageData, contentType);
    }

    public async Task<Image> UploadAsync(IFormFile image, bool isPrivate)
    {
        var optimizedImageStream = await ImageOptimizationUtil.Optimize(image, _imagesDefaultSize);

        var imageHash = HashByPhash(optimizedImageStream);
        var imageInDb = await _imagesRepository.GetAsync(imageHash);

        string imageExtension;
        imageExtension = Path.GetExtension(image.FileName);

        if (imageInDb != null)
        {
            if (!await _imagesRepository.IsAssignedToAnyQuizAsync(imageInDb.Id, null, isPrivate) && 
                !await _imagesRepository.IsAssignedToAnyQuestionAsync(imageInDb.Id, null, isPrivate))
            {
                await _blobStorageService.UploadAsync(optimizedImageStream, imageExtension, isPrivate, imageInDb.Id);
                await optimizedImageStream.DisposeAsync();
            }

            return imageInDb;
        }

        var imageName = await _blobStorageService.UploadAsync(optimizedImageStream, imageExtension, isPrivate);
        await optimizedImageStream.DisposeAsync();

        var newUploadedImage = new Image
        {
            Id = imageName,
            FileExtension = imageExtension,
            Hash = imageHash
        };

        await _repository.AddAsync(newUploadedImage);

        return newUploadedImage;
    }

    public async Task DeleteIfNotAssignedAsync(Guid imageId, bool isPrivate, Guid? quizIdToSkip = null, int? questionIdToSkip = null)
    {
        var imageExtension = await _repository.GetFieldAsync<Image, string>(imageId, "FileExtension") ?? 
            throw new NotFoundException($"Image with ID {imageId} was not found");

        if (!await _imagesRepository.IsAssignedToAnyQuizAsync(imageId, quizIdToSkip) && 
            !await _imagesRepository.IsAssignedToAnyQuestionAsync(imageId, questionIdToSkip))
        {
            await _repository.DeleteAsync<Image>(imageId);
            await _blobStorageService.DeleteAsync(imageId, imageExtension, isPrivate);
        }
        else if (!await _imagesRepository.IsAssignedToAnyQuizAsync(imageId, quizIdToSkip, isPrivate) && 
             !await _imagesRepository.IsAssignedToAnyQuestionAsync(imageId, questionIdToSkip, isPrivate))
        {
            await _blobStorageService.DeleteAsync(imageId, imageExtension, isPrivate);
        }
    }

    public async Task MoveImagesAsync(HashSet<(Guid imageId, string imageExtension)> images, bool isPrivate)
    {
        foreach (var (imageId, imageExtension) in images)
        {
            await _blobStorageService.CopyImageAsync(imageId, imageExtension, isPrivate);

            if (!await _imagesRepository.IsAssignedToAnyQuizAsync(imageId, null, isPrivate) && 
                !await _imagesRepository.IsAssignedToAnyQuestionAsync(imageId, null, isPrivate))
            {
                await _blobStorageService.DeleteAsync(imageId, imageExtension, isPrivate);
            }
        }
    }

    private byte[] HashByPhash(Stream imageStream)
    {
        var image = SixLabors.ImageSharp.Image.Load<L8>(imageStream);

        byte[] pixelData = new byte[image.Width * image.Height];
        image.ProcessPixelRows(accessor =>
        {
            for (int y = 0; y < image.Height; y++)
            {
                var row = accessor.GetRowSpan(y);
                for (int x = 0; x < image.Width; x++)
                {
                    pixelData[y * image.Width + x] = row[x].PackedValue;
                }
            }
        });

        var byteImage = new Common.ByteImage(pixelData, image.Width, image.Height);
        var hash = ImagePhash.ComputeDigest(byteImage);

        imageStream.Position = 0;

        return Convert.FromHexString(hash.ToString().AsSpan(2));
    }
}
