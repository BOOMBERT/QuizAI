using MediatR;
using QuizAI.Application.Interfaces;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Quizzes.Commands.DeleteQuizImage;

public class DeleteQuizImageCommandHandler : IRequestHandler<DeleteQuizImageCommand>
{
    private readonly IRepository _repository;
    private readonly IQuizzesRepository _quizzesRepository;
    private readonly IImageService _imageService;

    public DeleteQuizImageCommandHandler(IRepository repository, IQuizzesRepository quizzesRepository, IImageService imageService)
    {
        _repository = repository;
        _quizzesRepository = quizzesRepository;
        _imageService = imageService;
    }

    public async Task Handle(DeleteQuizImageCommand request, CancellationToken cancellationToken)
    {
        var quizId = request.GetId();

        if (!await _repository.EntityExistsAsync<Quiz>(quizId))
            throw new NotFoundException($"Quiz with ID {quizId} was not found");

        var imageId = await _quizzesRepository.GetImageIdAsync(quizId) ?? 
            throw new NotFoundException($"Quiz with ID {quizId} has no associated image.");

        await _quizzesRepository.UpdateImageAsync(quizId, null);
        await _imageService.DeleteIfNotAssignedAsync(imageId);
    }
}
