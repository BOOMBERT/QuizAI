using MediatR;
using QuizAI.Application.Interfaces;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Enums;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Quizzes.Queries.GetQuizImageById;

public class GetQuizImageByIdHandler : IRequestHandler<GetQuizImageByIdQuery, (byte[], string)>
{
    private readonly IRepository _repository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IQuizzesRepository _quizzesRepository;
    private readonly IImagesRepository _imagesRepository;

    public GetQuizImageByIdHandler(
        IRepository repository, IFileStorageService fileStorageService, IQuizzesRepository quizzesRepository, IImagesRepository imagesRepository)
    {
        _repository = repository;
        _fileStorageService = fileStorageService;
        _quizzesRepository = quizzesRepository;
        _imagesRepository = imagesRepository;
    }

    public async Task<(byte[], string)> Handle(GetQuizImageByIdQuery request, CancellationToken cancellationToken)
    {
        if (!await _repository.EntityExistsAsync<Quiz>(request.quizId))
            throw new NotFoundException($"Quiz with ID {request.quizId} was not found");

        var imageNameAsGuid = await _quizzesRepository.GetImageId(request.quizId) 
            ?? throw new NotFoundException($"Quiz with ID {request.quizId} has no associated image.");

        var imageExtension = await _imagesRepository.GetExtensionAsync(imageNameAsGuid)
            ?? throw new NotFoundException($"Image for quiz with ID {request.quizId} could not be found.");

        var imageData = await _fileStorageService.RetrieveFileAsync(imageNameAsGuid, imageExtension);

        if (imageExtension == FileExtension.Jpg) imageExtension = FileExtension.Jpeg;
        var contentType = "image/" + imageExtension.TrimStart('.');
        return (imageData, contentType);
    }
}
