﻿using MediatR;
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
    private readonly IQuizzesRepository _quizzesRepository;
    private readonly IQuestionsRepository _questionsRepository;
    private readonly (ushort width, ushort height)? _imagesDefaultSize;

    public ImageService(
        IRepository repository,
        IFileStorageService fileStorageService,
        IImagesRepository imagesRepository, 
        IQuizzesRepository quizzesRepository,
        IQuestionsRepository questionsRepository,
        (ushort width, ushort height)? imagesDefaultSize)
    {
        _repository = repository;
        _fileStorageService = fileStorageService;
        _imagesRepository = imagesRepository;
        _quizzesRepository = quizzesRepository;
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
            imageNameAsGuid = await _quizzesRepository.GetImageIdAsync(quizId) 
                ?? throw new NotFoundException($"Quiz with ID {quizId} has no associated image.");
        }
        else
        {
            imageNameAsGuid = await _questionsRepository.GetImageIdAsync(quizId, (int)questionId) 
                ?? throw new NotFoundException($"Question with ID {questionId} in quiz with ID {quizId} has no associated image.");

            questionErrorMessageContext = $"question with ID {questionId} in ";
        }

        var imageExtension = await _imagesRepository.GetExtensionAsync(imageNameAsGuid)
            ?? throw new NotFoundException($"Image for {questionErrorMessageContext}quiz with ID {quizId} could not be found.");

        var imageData = await _fileStorageService.RetrieveAsync(imageNameAsGuid, imageExtension);

        if (imageExtension == FileExtension.Jpg) imageExtension = FileExtension.Jpeg;
        var contentType = "image/" + imageExtension.TrimStart('.');
        return (imageData, contentType);
    }

    public async Task UpdateAsync(IFormFile image, Guid quizId, int? questionId = null)
    {
        if (!await _repository.EntityExistsAsync<Quiz>(quizId))
            throw new NotFoundException($"Quiz with ID {quizId} was not found");

        Guid? previousImageId;

        if (questionId == null)
        {
            previousImageId = await _quizzesRepository.GetImageIdAsync(quizId);
        }
        else
        {
            previousImageId = await _questionsRepository.GetImageIdAsync(quizId, (int)questionId);
        }

        var newUploadedImage = await UploadAsync(image);

        if (!await _repository.EntityExistsAsync<Image>(newUploadedImage.Id))
        {
            await _repository.AddAsync(newUploadedImage);
            await _repository.SaveChangesAsync();
        }

        if (questionId == null)
        {
            await _quizzesRepository.UpdateImageAsync(quizId, newUploadedImage.Id);
        }
        else
        {
            await _questionsRepository.UpdateImageAsync(quizId, (int)questionId, newUploadedImage.Id);
        }

        if (previousImageId != null)
            await DeleteIfNotAssignedAsync((Guid)previousImageId);
    }

    public async Task DeleteAsync(Guid quizId, int? questionId = null)
    {
        if (!await _repository.EntityExistsAsync<Quiz>(quizId))
            throw new NotFoundException($"Quiz with ID {quizId} was not found");

        Guid imageId;

        if (questionId == null)
        {
            imageId = await _quizzesRepository.GetImageIdAsync(quizId) 
                ?? throw new NotFoundException($"Quiz with ID {quizId} has no associated image.");

            await _quizzesRepository.UpdateImageAsync(quizId, null);
        }
        else
        {
            imageId = await _questionsRepository.GetImageIdAsync(quizId, (int)questionId) 
                ?? throw new NotFoundException($"Question with ID {questionId} in quiz with ID {quizId} has no associated image.");

            await _questionsRepository.UpdateImageAsync(quizId, (int)questionId, null);
        }

        await DeleteIfNotAssignedAsync(imageId);
    }

    public async Task<Image> UploadAsync(IFormFile image)
    {
        byte[] optimizedImage = await ImageOptimizationUtil.Optimize(image, _imagesDefaultSize);

        var imageHash = HashImageByPhash(optimizedImage);
        var imageInDb = await _imagesRepository.GetAsync(imageHash);

        if (imageInDb != null)
            return imageInDb;

        var imageExtension = Path.GetExtension(image.FileName);
        var imageName = await _fileStorageService.UploadAsync(optimizedImage, imageExtension);

        return new Image
        {
            Id = imageName,
            FileExtension = imageExtension,
            Hash = imageHash
        };
    }

    public async Task DeleteIfNotAssignedAsync(Guid imageId)
    {
        var imageExtension = await _imagesRepository.GetExtensionAsync(imageId) ?? 
            throw new NotFoundException($"Image with ID {imageId} was not found");

        if (!await _imagesRepository.IsAssignedToAnyQuizAsync(imageId) && !await _imagesRepository.IsAssignedToAnyQuestionAsync(imageId))
        {
            await _repository.DeleteAsync<Image>(imageId);
            _fileStorageService.Delete(imageId, imageExtension);
        }
    }

    private byte[] HashImageByPhash(byte[] imageBytes)
    {
        using (var bitmap = new System.Drawing.Bitmap(new MemoryStream(imageBytes)))
        {
            var luminanceImage = bitmap.ToLuminanceImage();

            var hash = ImagePhash.ComputeDigest(luminanceImage);
            return Convert.FromHexString(hash.ToString().AsSpan(2));
        }
    }
}
