using QuizAI.Application.QuizAttempts.Dtos;
using QuizAI.Domain.Entities;

namespace QuizAI.Application.Interfaces;

public interface IQuizAttemptService
{
    Task<QuizAttemptWithUserAnsweredQuestionsDto> GetWithUserAnsweredQuestionsAsync(QuizAttempt finishedQuizAttempt, int questionCount);
}
