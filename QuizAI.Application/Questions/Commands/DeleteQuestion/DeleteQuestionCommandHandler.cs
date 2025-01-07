using MediatR;
using QuizAI.Application.Interfaces;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Questions.Commands.DeleteQuestion;

public class DeleteQuestionCommandHandler : IRequestHandler<DeleteQuestionCommand>
{
    private readonly IRepository _repository;
    private readonly IQuestionsRepository _questionsRepository;
    private readonly IImageService _imageService;

    public DeleteQuestionCommandHandler(IRepository repository, IQuestionsRepository questionsRepository, IImageService imageService)
    {
        _repository = repository;
        _questionsRepository = questionsRepository;
        _imageService = imageService;
    }

    public async Task Handle(DeleteQuestionCommand request, CancellationToken cancellationToken)
    {
        var (quizId, questionId) = (request.GetQuizId(), request.GetQuestionId());

        if (!await _repository.EntityExistsAsync<Quiz>(quizId))
            throw new NotFoundException($"Quiz with ID {quizId} was not found");

        var imageId = await _questionsRepository.GetImageIdAsync(quizId, questionId);

        await _repository.DeleteAsync<Question>(questionId);

        if (imageId != null)
            await _imageService.DeleteIfNotAssignedAsync((Guid)imageId);
    }
}
