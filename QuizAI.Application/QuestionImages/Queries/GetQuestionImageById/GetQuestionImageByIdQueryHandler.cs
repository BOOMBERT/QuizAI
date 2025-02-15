using MediatR;
using QuizAI.Application.Interfaces;
using QuizAI.Domain.Constants;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.QuestionImages.Queries.GetQuestionImageById;

public class GetQuestionImageByIdQueryHandler : IRequestHandler<GetQuestionImageByIdQuery, (byte[], string)>
{
    private readonly IRepository _repository;
    private readonly IQuizAuthorizationService _quizAuthorizationService;
    private readonly IImageService _imageService;

    public GetQuestionImageByIdQueryHandler(IRepository repository, IQuizAuthorizationService quizAuthorizationService, IImageService imageService)
    {
        _repository = repository;
        _quizAuthorizationService = quizAuthorizationService;
        _imageService = imageService;
    }

    public async Task<(byte[], string)> Handle(GetQuestionImageByIdQuery request, CancellationToken cancellationToken)
    {
        var quiz = await _repository.GetEntityAsync<Quiz>(request.QuizId) 
            ?? throw new NotFoundException($"Quiz with ID {request.QuizId} was not found");

        await _quizAuthorizationService.AuthorizeAsync(quiz, null, ResourceOperation.Read);

        return await _imageService.GetDataToReturnAsync(quiz, request.QuestionId);
    }
}
