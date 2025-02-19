using QuizAI.Application.Interfaces;
using QuizAI.Application.Questions.Dtos;
using QuizAI.Application.QuizAttempts.Dtos;
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
}
