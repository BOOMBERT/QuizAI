using MediatR;
using QuizAI.Application.Interfaces;

namespace QuizAI.Application.Questions.Commands.UpdateQuestionImage;

public class UpdateQuestionImageCommandHandler : IRequestHandler<UpdateQuestionImageCommand>
{
    private readonly IImageService _imageService;

    public UpdateQuestionImageCommandHandler(IImageService imageService)
    {
        _imageService = imageService;
    }

    public async Task Handle(UpdateQuestionImageCommand request, CancellationToken cancellationToken)
    {
        await _imageService.UpdateAsync(request.Image, request.GetQuizId(), request.GetQuestionId());
    }
}
