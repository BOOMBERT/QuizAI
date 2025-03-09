using MediatR;
using QuizAI.Application.Interfaces;
using QuizAI.Application.QuizAttempts.Dtos;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.QuizAttempts.Queries.GetAttemptById;

public class GetAttemptByIdQueryHandler : IRequestHandler<GetAttemptByIdQuery, QuizAttemptViewWithUserAnsweredQuestionsDto>
{
    private readonly IUserContext _userContext;
    private readonly IQuizzesRepository _quizzesRepository;
    private readonly IQuizAttemptsRepository _quizAttemptsRepository;
    private readonly IQuizAttemptService _quizAttemptService;

    public GetAttemptByIdQueryHandler(
        IUserContext userContext, IQuizzesRepository quizzesRepository, IQuizAttemptsRepository quizAttemptsRepository, IQuizAttemptService quizAttemptService)
    {
        _userContext = userContext;
        _quizzesRepository = quizzesRepository;
        _quizAttemptsRepository = quizAttemptsRepository;
        _quizAttemptService = quizAttemptService;
    }

    public async Task<QuizAttemptViewWithUserAnsweredQuestionsDto> Handle(GetAttemptByIdQuery request, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.GetCurrentUser();

        var finishedQuizAttempt = await _quizAttemptsRepository.GetFinishedByIdAsync(request.QuizAttemptId, currentUser.Id)
            ?? throw new NotFoundException($"No finished quiz attempt found with ID {request.QuizAttemptId} for user with ID {currentUser.Id}");

        var quizNameAndQuestionCountAndIsPrivate = await _quizzesRepository.GetNameAndQuestionCountAndIsPrivateAsync(finishedQuizAttempt.QuizId);
        var (quizName, questionCount, isPrivate) = quizNameAndQuestionCountAndIsPrivate.Value;

        return await _quizAttemptService.GetWithUserAnsweredQuestionsAsync(finishedQuizAttempt, questionCount, quizName, isPrivate);
    }
}
