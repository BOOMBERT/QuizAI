using MediatR;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Services;

namespace QuizAI.Application.Questions.Commands.DeleteQuestionImage;

public class DeleteQuestionImageCommandHandler : IRequestHandler<DeleteQuestionImageCommand>
{
    private readonly IImageService _imageService;

    public DeleteQuestionImageCommandHandler(IImageService imageService)
    {
        _imageService = imageService;
    }

    public async Task Handle(DeleteQuestionImageCommand request, CancellationToken cancellationToken)
    {
        await _imageService.DeleteAsync(request.QuizId, request.QuestionId);
    }
}
