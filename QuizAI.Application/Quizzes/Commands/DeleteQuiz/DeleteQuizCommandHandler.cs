using MediatR;
using QuizAI.Application.Interfaces;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Quizzes.Commands.DeleteQuiz;

public class DeleteQuizCommandHandler : IRequestHandler<DeleteQuizCommand>
{
    private readonly IRepository _repository;
    private readonly IQuizzesRepository _quizzesRepository;
    private readonly IImageService _imageService;

    public DeleteQuizCommandHandler(IRepository repository, IQuizzesRepository quizzesRepository, IImageService imageService)
    {
        _repository = repository;
        _quizzesRepository = quizzesRepository;
        _imageService = imageService;
    }

    public async Task Handle(DeleteQuizCommand request, CancellationToken cancellationToken)
    {
        var quizId = request.GetId();

        if (!await _repository.EntityExistsAsync<Quiz>(quizId))
            throw new NotFoundException($"Quiz with ID {quizId} was not found");

        var imageId = await _quizzesRepository.GetImageIdAsync(quizId);
        
        await _repository.DeleteAsync<Quiz>(quizId);

        if (imageId != null)
            await _imageService.DeleteIfNotAssignedAsync((Guid)imageId);
    }
}
