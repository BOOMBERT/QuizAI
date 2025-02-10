using AutoMapper;
using MediatR;
using QuizAI.Application.Common;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Services;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Quizzes.Commands.DeleteQuizImage;

public class DeleteQuizImageCommandHandler : IRequestHandler<DeleteQuizImageCommand, LatestQuizId>
{
    private readonly IRepository _repository;
    private readonly IQuizService _quizService;
    private readonly IImageService _imageService;

    public DeleteQuizImageCommandHandler(IRepository repository, IQuizService quizService, IImageService imageService)
    {
        _repository = repository;
        _quizService = quizService;
        _imageService = imageService;
    }

    public async Task<LatestQuizId> Handle(DeleteQuizImageCommand request, CancellationToken cancellationToken)
    {
        var (quiz, createdNewQuiz) = await _quizService.GetValidOrDeprecateAndCreateAsync(request.GetId());

        var imageId = quiz.ImageId ?? throw new NotFoundException($"Quiz with ID {request.GetId()} has no associated image");
        quiz.ImageId = null;

        if (!createdNewQuiz)
        {
            await _imageService.DeleteIfNotAssignedAsync(imageId, quiz.Id);
        }
        else
        {
            await _repository.AddAsync(quiz);
        }

        await _repository.SaveChangesAsync();

        return new LatestQuizId(quiz.Id);
    }
}
