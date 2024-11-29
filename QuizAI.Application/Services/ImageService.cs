﻿using Microsoft.AspNetCore.Http;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Utils;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Repositories;
using Shipwreck.Phash;
using Shipwreck.Phash.Bitmaps;

namespace QuizAI.Application.Services;

public class ImageService : IImageService
{
    private readonly IImagesRepository _imagesRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly (ushort width, ushort height)? _imagesDefaultSize;
    private readonly int _imagesMaxSizeInBytes;

    public ImageService(
        IImagesRepository imagesRepository, IFileStorageService fileStorageService, (ushort width, ushort height)? imagesDefaultSize, int imagesMaxSizeInBytes)
    {
        _imagesRepository = imagesRepository;
        _fileStorageService = fileStorageService;
        _imagesDefaultSize = imagesDefaultSize;
        _imagesMaxSizeInBytes = imagesMaxSizeInBytes;
    }

    public async Task<Image> UploadAsync(IFormFile image)
    {
        FileValidationUtil.Validate(image, _imagesMaxSizeInBytes);
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
