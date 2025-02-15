using MediatR;
using QuizAI.Application.Interfaces;
using QuizAI.Application.QuizAttempts.Dtos;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.QuizAttempts.Queries.GetLatestAttempt;

public class GetLatestAttemptQueryHandler : IRequestHandler<GetLatestAttemptQuery, QuizAttemptViewWithUserAnsweredQuestionsDto>
{
    private readonly IUserContext _userContext;
    private readonly IQuizzesRepository _quizzesRepository;
    private readonly IQuizAttemptsRepository _quizAttemptsRepository;
    private readonly IQuizAttemptService _quizAttemptService;

    public GetLatestAttemptQueryHandler(
        IUserContext userContext, IQuizzesRepository quizzesRepository, IQuizAttemptsRepository quizAttemptsRepository, IQuizAttemptService quizAttemptService)
    {
        _userContext = userContext;
        _quizzesRepository = quizzesRepository;
        _quizAttemptsRepository = quizAttemptsRepository;
        _quizAttemptService = quizAttemptService;
    }

    public async Task<QuizAttemptViewWithUserAnsweredQuestionsDto> Handle(GetLatestAttemptQuery request, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.GetCurrentUser();

        var (quizName, questionCount) = await _quizzesRepository.GetNameAndQuestionCountAsync(request.QuizId)
            ?? throw new NotFoundException($"Quiz with ID {request.QuizId} was not found");

        var latestFinishedAttempt = await _quizAttemptsRepository.GetLatestFinishedAsync(request.QuizId, currentUser.Id)
            ?? throw new NotFoundException($"No finished quiz attempt found for quiz with ID {request.QuizId} and user with ID {currentUser.Id}");

        return await _quizAttemptService.GetWithUserAnsweredQuestionsAsync(latestFinishedAttempt, questionCount, quizName);
    }
}
