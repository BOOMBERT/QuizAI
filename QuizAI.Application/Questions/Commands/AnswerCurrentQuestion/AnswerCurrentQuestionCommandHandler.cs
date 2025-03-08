using MediatR;
using QuizAI.Application.Interfaces;
using QuizAI.Domain.Constants;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Questions.Commands.AnswerCurrentQuestion;

public class AnswerCurrentQuestionCommandHandler : IRequestHandler<AnswerCurrentQuestionCommand>
{
    private readonly IRepository _repository;
    private readonly IUserContext _userContext;
    private readonly IQuizAuthorizationService _quizAuthorizationService;
    private readonly IQuestionsRepository _questionsRepository;
    private readonly IQuizAttemptsRepository _quizAttemptsRepository;
    private readonly IQuizAttemptService _quizAttemptService;
    private readonly IAnswerService _answerService;

    public AnswerCurrentQuestionCommandHandler(
        IRepository repository, IUserContext userContext, IQuizAuthorizationService quizAuthorizationService, 
        IQuestionsRepository questionsRepository, IQuizAttemptsRepository quizAttemptsRepository, IQuizAttemptService quizAttemptService, 
        IAnswerService answerService)
    {
        _repository = repository;
        _userContext = userContext;
        _quizAuthorizationService = quizAuthorizationService;
        _questionsRepository = questionsRepository;
        _quizAttemptsRepository = quizAttemptsRepository;
        _quizAttemptService = quizAttemptService;
        _answerService = answerService;
    }

    public async Task Handle(AnswerCurrentQuestionCommand request, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.GetCurrentUser();

        var quiz = await _repository.GetEntityAsync<Quiz>(request.GetQuizId()) 
            ?? throw new NotFoundException($"Quiz with ID {request.GetQuizId()} was not found");

        await _quizAuthorizationService.AuthorizeAsync(quiz, currentUser.Id, ResourceOperation.Create);

        var unfinishedAttempt = await _quizAttemptsRepository.GetUnfinishedAsync(request.GetQuizId(), currentUser.Id)
            ?? throw new ConflictException(
                $"User with ID {currentUser.Id} does not have any unfinished attempt for quiz with ID {request.GetQuizId()} - " +
                $"Please start a new attempt");

        var questionToAnswer = await _questionsRepository.GetByOrderAsync(request.GetQuizId(), unfinishedAttempt.CurrentOrder, true);

        _answerService.ValidateUserAnswer(request.UserAnswer, questionToAnswer!.Type);
        var isUserAnswerCorrect = await _answerService.CheckUserAnswerAsync(request.UserAnswer, questionToAnswer);

        var userAnswer = new UserAnswer
        {
            QuizAttemptId = unfinishedAttempt.Id,
            QuestionId = questionToAnswer.Id,
            AnswerText = request.UserAnswer,
            IsCorrect = isUserAnswerCorrect
        };

        if (isUserAnswerCorrect) unfinishedAttempt.CorrectAnswers++;

        var isLastQuestion = quiz.QuestionCount == unfinishedAttempt.CurrentOrder;
        if (isLastQuestion)
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

        if (isLastQuestion)
            await _quizAttemptService.CheckAndSendAchievementForNumberOfAttemptsAsync(quiz);
    }
}
