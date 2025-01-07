using MediatR;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Services;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Questions.Commands.DeleteQuestion;

public class DeleteQuestionCommandHandler : IRequestHandler<DeleteQuestionCommand>
{
    private readonly IRepository _repository;
    private readonly IQuestionsRepository _questionsRepository;
    private readonly IQuestionService _questionService;
    private readonly IImageService _imageService;

    public DeleteQuestionCommandHandler(IRepository repository, IQuestionsRepository questionsRepository, IQuestionService questionService, IImageService imageService)
    {
        _repository = repository;
        _questionsRepository = questionsRepository;
        _questionService = questionService;
        _imageService = imageService;
    }

    public async Task Handle(DeleteQuestionCommand request, CancellationToken cancellationToken)
    {
        var imageId = await _questionsRepository.GetImageIdAsync(request.QuizId, request.QuestionId);

        await _questionService.DeleteAsync(request.QuizId, request.QuestionId);
        await _repository.SaveChangesAsync();

        if (imageId != null)
            await _imageService.DeleteIfNotAssignedAsync((Guid)imageId);
    }
}
