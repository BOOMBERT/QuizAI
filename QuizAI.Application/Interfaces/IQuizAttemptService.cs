using QuizAI.Application.QuizAttempts.Dtos;
using QuizAI.Domain.Entities;

namespace QuizAI.Application.Interfaces;

public interface IQuizAttemptService
{
    Task<QuizAttemptViewWithUserAnsweredQuestionsDto> GetWithUserAnsweredQuestionsAsync(QuizAttempt finishedQuizAttempt, int questionCount, string quizName, bool isPrivate);
    Task CheckAndSendAchievementForNumberOfAttemptsAsync(Quiz quiz);
}
