using QuizAI.Application.Interfaces;
using QuizAI.Application.Questions.Dtos;
using QuizAI.Application.Quizzes.Dtos;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Services;

public class QuizAttemptService : IQuizAttemptService
{
    private readonly IAnswersRepository _answersRepository;
    private readonly IQuestionService _questionService;

    public QuizAttemptService(IAnswersRepository answersRepository, IQuestionService questionService)
    {
        _answersRepository = answersRepository;
        _questionService = questionService;
    }

    public async Task<QuizAttemptWithUserAnsweredQuestionsDto> GetWithUserAnsweredQuestionsAsync(QuizAttempt finishedQuizAttempt, int questionCount)
    {
        var userAnswersWithQuestions = await _answersRepository.GetUserAnswersByAttemptIdAsync(finishedQuizAttempt.Id, true);

        var quizAttemptWithUserAnswers = new List<UserAnsweredQuestionDto>();

        foreach (var userAnswersWithQuestion in userAnswersWithQuestions)
        {
            quizAttemptWithUserAnswers.Add(
                new UserAnsweredQuestionDto(
                    _questionService.MapToQuestionWithAnswersDto(userAnswersWithQuestion.Question),
                    new UserAnswerDto(userAnswersWithQuestion.Id, userAnswersWithQuestion.AnswerText, userAnswersWithQuestion.IsCorrect, userAnswersWithQuestion.AnsweredAt)
                )
            );
        }

        return new QuizAttemptWithUserAnsweredQuestionsDto(
            quizAttemptWithUserAnswers, finishedQuizAttempt.CorrectAnswers, questionCount, finishedQuizAttempt.StartedAt, (DateTime)finishedQuizAttempt.FinishedAt!);
    }
}
