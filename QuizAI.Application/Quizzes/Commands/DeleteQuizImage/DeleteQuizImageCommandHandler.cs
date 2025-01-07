using MediatR;
using QuizAI.Application.Interfaces;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Quizzes.Commands.DeleteQuizImage;

public class DeleteQuizImageCommandHandler : IRequestHandler<DeleteQuizImageCommand>
{
    private readonly IImageService _imageService;

    public DeleteQuizImageCommandHandler(IImageService imageService)
    {
        _imageService = imageService;
    }

    public async Task Handle(DeleteQuizImageCommand request, CancellationToken cancellationToken)
    {
        await _imageService.DeleteAsync(request.GetId());
    }
}
