using MediatR;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Services;

namespace QuizAI.Application.Questions.Queries.GetQuestionImageById;

public class GetQuestionImageByIdQueryHandler : IRequestHandler<GetQuestionImageByIdQuery, (byte[], string)>
{
    private readonly IImageService _imageService;

    public GetQuestionImageByIdQueryHandler(IImageService imageService)
    {
        _imageService = imageService;
    }

    public async Task<(byte[], string)> Handle(GetQuestionImageByIdQuery request, CancellationToken cancellationToken)
    {
        return await _imageService.GetDataToReturnAsync(request.QuizId, request.QuestionId);
    }
}
