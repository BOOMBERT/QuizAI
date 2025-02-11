using MediatR;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Services;
using QuizAI.Application.Users;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Enums;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Questions.Commands.AnswerCurrentQuestion;

public class AnswerCurrentQuestionCommandHandler : IRequestHandler<AnswerCurrentQuestionCommand>
{
    private readonly IRepository _repository;
    private readonly IUserContext _userContext;
    private readonly IQuizzesRepository _quizzesRepository;
    private readonly IQuestionsRepository _questionsRepository;
    private readonly IQuizAttemptsRepository _quizAttemptsRepository;
    private readonly IAnswerService _answerService;

    public AnswerCurrentQuestionCommandHandler(
        IRepository repository, IUserContext userContext, 
        IQuizzesRepository quizzesRepository, IQuestionsRepository questionsRepository, IQuizAttemptsRepository quizAttemptsRepository, IAnswerService answerService)
    {
        _repository = repository;
        _userContext = userContext;
        _quizzesRepository = quizzesRepository;
        _questionsRepository = questionsRepository;
        _quizAttemptsRepository = quizAttemptsRepository;
        _answerService = answerService;
    }

    public async Task Handle(AnswerCurrentQuestionCommand request, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.GetCurrentUser();

        var questionCount = await _quizzesRepository.GetQuestionCountAsync(request.GetQuizId()) 
            ?? throw new NotFoundException($"Quiz with ID {request.GetQuizId()} was not found");

        var unfinishedAttempt = await _quizAttemptsRepository.GetUnfinishedAsync(request.GetQuizId(), currentUser.Id)
            ?? throw new ConflictException(
                $"User with ID {currentUser.Id} does not have any unfinished attempt for quiz with ID {request.GetQuizId()} - " +
                $"Please start a new attempt");

        var questionToAnswer = await _questionsRepository.GetByOrderAsync(request.GetQuizId(), unfinishedAttempt.CurrentOrder, true);

        _answerService.ValidateUserAnswer(request.UserAnswer, questionToAnswer!.Type);
        var isUserAnswerCorrect = _answerService.CheckUserAnswer(request.UserAnswer, questionToAnswer);

        var userAnswer = new UserAnswer
        {
            QuizAttemptId = unfinishedAttempt.Id,
            QuestionId = questionToAnswer.Id,
            AnswerText = request.UserAnswer,
            IsCorrect = isUserAnswerCorrect
        };

        if (isUserAnswerCorrect) unfinishedAttempt.CorrectAnswers++;

        if (questionCount == unfinishedAttempt.CurrentOrder)
        {
            unfinishedAttempt.CurrentOrder = 0;
            unfinishedAttempt.FinishedAt = DateTime.UtcNow;
        }
        else
        {
            unfinishedAttempt.CurrentOrder++;
        }

        await _repository.AddAsync(userAnswer);
        await _repository.SaveChangesAsync();
    }
}
