using MediatR;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Questions.Dtos;
using QuizAI.Application.Quizzes.Dtos;
using QuizAI.Application.Services;
using QuizAI.Application.Users;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Quizzes.Queries.GetAttemptById;

public class GetAttemptByIdQueryHandler : IRequestHandler<GetAttemptByIdQuery, QuizAttemptWithUserAnsweredQuestionsDto>
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

    public async Task<QuizAttemptWithUserAnsweredQuestionsDto> Handle(GetAttemptByIdQuery request, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.GetCurrentUser();

        var questionCount = await _quizzesRepository.GetQuestionCountAsync(request.QuizId) 
            ?? throw new NotFoundException($"Quiz with ID {request.QuizId} was not found");

        var finishedQuizAttempt = await _quizAttemptsRepository.GetFinishedByIdAsync(request.QuizId, request.QuizAttemptId, currentUser.Id)
            ?? throw new NotFoundException($"Quiz with ID {request.QuizId} does not have a finished attempt with ID {request.QuizAttemptId} for user with ID {currentUser.Id}");

        return await _quizAttemptService.GetWithUserAnsweredQuestionsAsync(finishedQuizAttempt, questionCount);
    }
}
