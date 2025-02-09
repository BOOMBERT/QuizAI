using MediatR;
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
    private readonly IQuestionsRepository _questionsRepository;
    private readonly (ushort width, ushort height)? _imagesDefaultSize;

    public ImageService(
        IRepository repository,
        IFileStorageService fileStorageService,
        IImagesRepository imagesRepository, 
        IQuestionsRepository questionsRepository,
        (ushort width, ushort height)? imagesDefaultSize)
    {
        _repository = repository;
        _fileStorageService = fileStorageService;
        _imagesRepository = imagesRepository;
        _questionsRepository = questionsRepository;
        _imagesDefaultSize = imagesDefaultSize;
    }

    public async Task<(byte[], string)> GetDataToReturnAsync(Guid quizId, int? questionId = null)
    {
        if (!await _repository.EntityExistsAsync<Quiz>(quizId))
            throw new NotFoundException($"Quiz with ID {quizId} was not found");

        Guid imageNameAsGuid;
        string questionErrorMessageContext = string.Empty;

        if (questionId == null)
        {
            imageNameAsGuid = await _repository.GetFieldAsync<Quiz, Guid?>(quizId, "ImageId")
                ?? throw new NotFoundException($"Quiz with ID {quizId} has no associated image.");
        }
        else
        {
            imageNameAsGuid = await _questionsRepository.GetImageIdAsync(quizId, (int)questionId) 
                ?? throw new NotFoundException($"Question with ID {questionId} in quiz with ID {quizId} has no associated image.");

            questionErrorMessageContext = $"question with ID {questionId} in ";
        }

        var imageExtension = await _repository.GetFieldAsync<Image, string>(imageNameAsGuid, "FileExtension")
            ?? throw new NotFoundException($"Image for {questionErrorMessageContext}quiz with ID {quizId} could not be found.");

        var imageData = await _fileStorageService.RetrieveAsync(imageNameAsGuid, imageExtension);

        if (imageExtension == FileExtension.Jpg) imageExtension = FileExtension.Jpeg;
        var contentType = "image/" + imageExtension.TrimStart('.');
        return (imageData, contentType);
    }

    public async Task<Image> UploadAsync(IFormFile image)
    {
        byte[] optimizedImage = await ImageOptimizationUtil.Optimize(image, _imagesDefaultSize);

        var imageHash = HashByPhash(optimizedImage);
        var imageInDb = await _imagesRepository.GetAsync(imageHash);

        if (imageInDb != null)
            return imageInDb;

        var imageExtension = Path.GetExtension(image.FileName);
        var imageName = await _fileStorageService.UploadAsync(optimizedImage, imageExtension);

        var newUploadedImage = new Image
        {
            Id = imageName,
            FileExtension = imageExtension,
            Hash = imageHash
        };

        await _repository.AddAsync(newUploadedImage);

        return newUploadedImage;
    }

    public async Task DeleteIfNotAssignedAsync(Guid imageId, Guid? quizIdToSkip = null, int? questionIdToSkip = null)
    {
        var imageExtension = await _repository.GetFieldAsync<Image, string>(imageId, "FileExtension") ?? 
            throw new NotFoundException($"Image with ID {imageId} was not found");

        if (!await _imagesRepository.IsAssignedToAnyQuizAsync(imageId, quizIdToSkip) && 
            !await _imagesRepository.IsAssignedToAnyQuestionAsync(imageId, questionIdToSkip))
        {
            await _repository.DeleteAsync<Image>(imageId);
            _fileStorageService.Delete(imageId, imageExtension);
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
