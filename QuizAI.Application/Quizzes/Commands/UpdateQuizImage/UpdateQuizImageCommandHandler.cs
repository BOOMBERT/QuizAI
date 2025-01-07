using MediatR;
using QuizAI.Application.Interfaces;

namespace QuizAI.Application.Quizzes.Commands.UpdateQuizImage;

public class UpdateQuizImageCommandHandler : IRequestHandler<UpdateQuizImageCommand>
{
    private readonly IImageService _imageService;

    public UpdateQuizImageCommandHandler(IImageService imageService)
    {
        _imageService = imageService;
    }

    public async Task Handle(UpdateQuizImageCommand request, CancellationToken cancellationToken)
    {
        await _imageService.UpdateAsync(request.Image, request.GetId());
    }
}
