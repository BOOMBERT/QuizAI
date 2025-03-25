using Microsoft.AspNetCore.Http;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Questions.Dtos;
using QuizAI.Application.QuizAttempts.Dtos;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Interfaces;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Services;

public class QuizAttemptService : IQuizAttemptService
{
    private readonly IAnswersRepository _answersRepository;
    private readonly IQuestionService _questionService;
    private readonly IUserRepository _userRepository;
    private readonly IQuizAttemptsRepository _quizAttemptsRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IRabbitMqService _rabbitMqService;
    private readonly Dictionary<int, (string message, string subject)> achievementForNumberOfAttemptsMessagesAndSubjects = new Dictionary<int, (string, string)>
    {
        { 5, ("Your quiz '<strong>{0}</strong>' has been attempted 5 times! Great start!", "Your quiz is gaining popularity!") },
        { 25, ("Your quiz '<strong>{0}</strong>' has been attempted 25 times! It's gaining traction!", "Your quiz is gaining traction!") },
        { 50, ("Your quiz '<strong>{0}</strong>' has been attempted 50 times! It's becoming popular!", "Your quiz is becoming popular!") },
        { 100, ("Your quiz '<strong>{0}</strong>' has reached 100 attempts! Amazing engagement!", "Your quiz is amazing!") },
        { 250, ("Your quiz '<strong>{0}</strong>' has been attempted 250 times! Excellent reach!", "Your quiz has excellent reach!") },
        { 500, ("Your quiz '<strong>{0}</strong>' has crossed 500 attempts! It's a big hit!", "Your quiz is a big hit!") },
        { 1000, ("Your quiz '<strong>{0}</strong>' has been attempted 1,000 times! You've created a viral quiz!", "Your quiz is viral!") }
    };

    public QuizAttemptService(
        IAnswersRepository answersRepository, IQuestionService questionService, IUserRepository userRepository, 
        IQuizAttemptsRepository quizAttemptsRepository, IHttpContextAccessor httpContextAccessor, IRabbitMqService rabbitMqService)
    {
        _answersRepository = answersRepository;
        _questionService = questionService;
        _userRepository = userRepository;
        _quizAttemptsRepository = quizAttemptsRepository;
        _httpContextAccessor = httpContextAccessor;
        _rabbitMqService = rabbitMqService;
    }

    public async Task<QuizAttemptViewWithUserAnsweredQuestionsDto> GetWithUserAnsweredQuestionsAsync(
        QuizAttempt finishedQuizAttempt, int questionCount, string quizName, bool isPrivate)
    {
        var userAnswersWithQuestions = await _answersRepository.GetUserAnswersByAttemptIdAsync(finishedQuizAttempt.Id, true);

        var quizAttemptWithUserAnswers = new List<UserAnsweredQuestionDto>();

        foreach (var userAnswersWithQuestion in userAnswersWithQuestions)
        {
            quizAttemptWithUserAnswers.Add(
                new UserAnsweredQuestionDto(
                    await _questionService.MapToQuestionWithAnswersDtoAsync(userAnswersWithQuestion.Question, isPrivate),
                    new UserAnswerDto(userAnswersWithQuestion.Id, userAnswersWithQuestion.AnswerText, userAnswersWithQuestion.IsCorrect, userAnswersWithQuestion.AnsweredAt)
                )
            );
        }

        return new QuizAttemptViewWithUserAnsweredQuestionsDto(
            quizAttemptWithUserAnswers,
            new QuizAttemptViewDto(
                finishedQuizAttempt.Id,
                finishedQuizAttempt.QuizId,
                finishedQuizAttempt.StartedAt,
                (DateTime)finishedQuizAttempt.FinishedAt!,
                finishedQuizAttempt.CorrectAnswers, 
                questionCount,
                quizName
                ));
    }

    public async Task CheckAndSendAchievementForNumberOfAttemptsAsync(Quiz quiz)
    {
        var creatorEmail = await _userRepository.GetEmailAsync(quiz.CreatorId)
            ?? throw new NotFoundException($"User with ID {quiz.CreatorId} was not found");

        var finishedAttemptsCount = await _quizAttemptsRepository.HowManyAsync(quiz.Id, true);

        if (!achievementForNumberOfAttemptsMessagesAndSubjects.TryGetValue(finishedAttemptsCount, out var achievementResult))
            return;

        var (achievementMessage, achievementSubject) = (string.Format(achievementResult.message, quiz.Name), achievementResult.subject);

        var httpContextRequest = _httpContextAccessor.HttpContext!.Request;
        var getQuizLink = $"{httpContextRequest.Scheme}://{httpContextRequest.Host}/api/quizzes/{quiz.Id}";

        var (quizAttemptsCount, averageCorrectAnswers, averageTimeSpent) =
            await _quizAttemptsRepository.GetDetailedStatsAsync(quiz.Id, true);

        await _rabbitMqService.SendEmailToQueueAsync(
            toEmail: creatorEmail,
            subject: achievementSubject,
            htmlMessage: $"""
                <h1 style="color: #1eb3d7;"><strong>CONGRATULATIONS!!!</strong></h1><br>
                <h2 style="color: #ac47bd;">{achievementMessage}</h2>
                <h2 style="color: #ac47bd;"><a href="{getQuizLink}">Click here to view your quiz</a></h2><br>
                <h3 style="color: #1eb3d7;"><strong>Quiz Statistics:</strong></h3>
                <h3 style="color: #ac47bd;">- Total Attempts: {quizAttemptsCount}</h3>
                <h3 style="color: #ac47bd;">- Average Correct Answers: {averageCorrectAnswers*100:F2}%</h3>
                <h3 style="color: #ac47bd;">- Average Time Spent: {averageTimeSpent}</h3>
                """);
    }
}
