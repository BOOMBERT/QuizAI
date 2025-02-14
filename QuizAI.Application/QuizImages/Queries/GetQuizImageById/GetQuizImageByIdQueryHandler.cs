using MediatR;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Services;
using QuizAI.Domain.Constants;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.QuizImages.Queries.GetQuizImageById;

public class GetQuizImageByIdQueryHandler : IRequestHandler<GetQuizImageByIdQuery, (byte[], string)>
{
    private readonly IRepository _repository;
    private readonly IQuizAuthorizationService _quizAuthorizationService;
    private readonly IImageService _imageService;

    public GetQuizImageByIdQueryHandler(IRepository repository, IQuizAuthorizationService quizAuthorizationService, IImageService imageService)
    {
        _repository = repository;
        _quizAuthorizationService = quizAuthorizationService;
        _imageService = imageService;
    }

    public async Task<(byte[], string)> Handle(GetQuizImageByIdQuery request, CancellationToken cancellationToken)
    {
        var quiz = await _repository.GetEntityAsync<Quiz>(request.QuizId) 
            ?? throw new NotFoundException($"Quiz with ID {request.QuizId} was not found");
        
        await _quizAuthorizationService.AuthorizeAsync(quiz, null, ResourceOperation.Read);

        return await _imageService.GetDataToReturnAsync(request.QuizId);
    }
}
