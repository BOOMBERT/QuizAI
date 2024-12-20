using MediatR;
using QuizAI.Application.Interfaces;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Quizzes.Commands.UpdateQuizImage;

public class UpdateQuizImageCommandHandler : IRequestHandler<UpdateQuizImageCommand>
{
    private readonly IRepository _repository;
    private readonly IImageService _imageService;
    private readonly IImagesRepository _imageRepository;
    private readonly IQuizzesRepository _quizzesRepository;

    public UpdateQuizImageCommandHandler(IRepository repository, IImageService imageService, IImagesRepository imagesRepository, IQuizzesRepository quizzesRepository)
    {
        _repository = repository;
        _imageService = imageService;
        _imageRepository = imagesRepository;
        _quizzesRepository = quizzesRepository;
    }

    public async Task Handle(UpdateQuizImageCommand request, CancellationToken cancellationToken)
    {
        var quizId = request.GetId();

        if (!await _repository.EntityExistsAsync<Quiz>(quizId))
            throw new NotFoundException($"Quiz with ID {quizId} was not found");

        var previousImageId = await _quizzesRepository.GetImageIdAsync(quizId);
        var newUploadedImage = await _imageService.UploadAsync(request.Image);

        if (!await _repository.EntityExistsAsync<Image>(newUploadedImage.Id))
        {
            await _repository.AddAsync(newUploadedImage);
            await _repository.SaveChangesAsync();
        }

        await _quizzesRepository.UpdateImageAsync(quizId, newUploadedImage.Id);

        if (previousImageId != null)
            await _imageService.DeleteIfNotAssigned((Guid)previousImageId);
    }
}
