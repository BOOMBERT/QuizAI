using AutoMapper;
using MediatR;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Quizzes.Dtos;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Quizzes.Queries.GetQuizById;

public class GetQuizByIdQueryHandler : IRequestHandler<GetQuizByIdQuery, QuizDto>
{
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;
    private readonly IQuizAuthorizationService _quizAuthorizationService;
    private readonly IQuizzesRepository _quizzesRepository;
    private readonly IQuizAttemptsRepository _quizAttemptsRepository;
    private readonly IImagesRepository _imagesRepository;

    public GetQuizByIdQueryHandler(
        IMapper mapper, IUserContext userContext, IQuizAuthorizationService quizAuthorizationService, IQuizzesRepository quizzesRepository, 
        IQuizAttemptsRepository quizAttemptsRepository, IImagesRepository imagesRepository)
    {
        _mapper = mapper;
        _userContext = userContext;
        _quizAuthorizationService = quizAuthorizationService;
        _quizzesRepository = quizzesRepository;
        _quizAttemptsRepository = quizAttemptsRepository;
        _imagesRepository = imagesRepository;
    }

    public async Task<QuizDto> Handle(GetQuizByIdQuery request, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.GetCurrentUser();

        var quiz = await _quizzesRepository.GetAsync(request.QuizId, true, false, false) 
            ?? throw new NotFoundException($"Quiz with ID {request.QuizId} was not found");

        var canUserEditQuiz = await _quizAuthorizationService.AuthorizeReadOperationAndGetCanEditAsync(quiz, currentUser.Id);

        var hasUnfinishedAttempt = await _quizAttemptsRepository.HasAnyAsync(quiz.Id, currentUser.Id, false);

        string? publicImageUrl = null;

        if (!quiz.IsPrivate && quiz.ImageId != null)
            publicImageUrl = $"/api/uploads/{quiz.ImageId}{await _imagesRepository.GetFileExtensionAsync((Guid)quiz.ImageId)}";

        var quizDto = _mapper.Map<QuizDto>(quiz);

        return quizDto with { 
            CanEdit = canUserEditQuiz, 
            HasUnfinishedAttempt = hasUnfinishedAttempt,
            PublicImageUrl = publicImageUrl
        };
    }
}
