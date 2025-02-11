﻿using MediatR;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Questions.Dtos;
using QuizAI.Application.Quizzes.Dtos;
using QuizAI.Application.Services;
using QuizAI.Application.Users;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Quizzes.Queries.GetLatestAttempt;

public class GetLatestAttemptQueryHandler : IRequestHandler<GetLatestAttemptQuery, QuizAttemptWithUserAnsweredQuestionsDto>
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

    public async Task<QuizAttemptWithUserAnsweredQuestionsDto> Handle(GetLatestAttemptQuery request, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.GetCurrentUser();

        var questionCount = await _quizzesRepository.GetQuestionCountAsync(request.QuizId)
            ?? throw new NotFoundException($"Quiz with ID {request.QuizId} was not found");

        var latestFinishedAttempt = await _quizAttemptsRepository.GetLatestFinishedAsync(request.QuizId, currentUser.Id)
            ?? throw new NotFoundException($"Quiz with ID {request.QuizId} does not have any finished attempts");

        return await _quizAttemptService.GetWithUserAnsweredQuestionsAsync(latestFinishedAttempt, questionCount);
    }
}
