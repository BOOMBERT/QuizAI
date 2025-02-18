using Microsoft.AspNetCore.Http;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Utils;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Enums;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;
using Shipwreck.Phash;
using Shipwreck.Phash.Bitmaps;

namespace QuizAI.Application.Services;

public class ImageService : IImageService
{
    private readonly IRepository _repository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IImagesRepository _imagesRepository;
    private readonly (ushort width, ushort height)? _imagesDefaultSize;

    public ImageService(IRepository repository, IFileStorageService fileStorageService, IImagesRepository imagesRepository, (ushort width, ushort height)? imagesDefaultSize)
    {
        _repository = repository;
        _fileStorageService = fileStorageService;
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

        var imageData = await _fileStorageService.RetrieveAsync(imageName, imageExtension, quiz.IsPrivate);

        if (imageExtension == FileExtension.Jpg) imageExtension = FileExtension.Jpeg;
        var contentType = "image/" + imageExtension.TrimStart('.');
        return (imageData, contentType);
    }

    public async Task<Image> UploadAsync(IFormFile image, bool isPrivate)
    {
        byte[] optimizedImage = await ImageOptimizationUtil.Optimize(image, _imagesDefaultSize);

        var imageHash = HashByPhash(optimizedImage);
        var imageInDb = await _imagesRepository.GetAsync(imageHash);

        string imageExtension;
        imageExtension = Path.GetExtension(image.FileName);

        if (imageInDb != null)
        {
            if(!await _imagesRepository.IsInStorageAsync(imageHash, isPrivate))
                await _fileStorageService.UploadAsync(optimizedImage, imageExtension, isPrivate, imageInDb.Id);

            return imageInDb;
        }

        var imageName = await _fileStorageService.UploadAsync(optimizedImage, imageExtension, isPrivate);

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
            _fileStorageService.Delete(imageId, imageExtension, isPrivate);
        }
        else if (!await _imagesRepository.IsAssignedToAnyQuizAsync(imageId, quizIdToSkip, isPrivate) && 
             !await _imagesRepository.IsAssignedToAnyQuestionAsync(imageId, questionIdToSkip, isPrivate))
        {
            _fileStorageService.Delete(imageId, imageExtension, isPrivate);
        }
    }

    public async Task MoveImagesAsync(HashSet<(Guid imageId, string imageExtension)> images, bool isPrivate, bool doNotDelete = false)
    {
        foreach (var (imageId, imageExtension) in images)
        {
            _fileStorageService.CopyImage(imageId, imageExtension, isPrivate);

            if (!doNotDelete &&
                !await _imagesRepository.IsAssignedToAnyQuizAsync(imageId, null, isPrivate) && 
                !await _imagesRepository.IsAssignedToAnyQuestionAsync(imageId, null, isPrivate))
            {
                _fileStorageService.Delete(imageId, imageExtension, isPrivate);
            }
        }
    }

    private byte[] HashByPhash(byte[] imageBytes)
    {
        using (var bitmap = new System.Drawing.Bitmap(new MemoryStream(imageBytes)))
        {
            var luminanceImage = bitmap.ToLuminanceImage();

            var hash = ImagePhash.ComputeDigest(luminanceImage);
            return Convert.FromHexString(hash.ToString().AsSpan(2));
        }
    }
}
