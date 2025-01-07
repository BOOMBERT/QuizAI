using MediatR;
using QuizAI.Application.Interfaces;

namespace QuizAI.Application.Quizzes.Queries.GetQuizImageById;

public class GetQuizImageByIdQueryHandler : IRequestHandler<GetQuizImageByIdQuery, (byte[], string)>
{
    private readonly IImageService _imageService;

    public GetQuizImageByIdQueryHandler(IImageService imageService)
    {
        _imageService = imageService;
    }

    public async Task<(byte[], string)> Handle(GetQuizImageByIdQuery request, CancellationToken cancellationToken)
    {
        return await _imageService.GetDataToReturnAsync(request.QuizId);
    }
}
