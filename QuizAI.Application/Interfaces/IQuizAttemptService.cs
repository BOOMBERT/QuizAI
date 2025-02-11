using QuizAI.Application.Quizzes.Dtos;
using QuizAI.Domain.Entities;

namespace QuizAI.Application.Interfaces;

public interface IQuizAttemptService
{
    Task<QuizAttemptWithUserAnsweredQuestionsDto> GetWithUserAnsweredQuestionsAsync(QuizAttempt finishedQuizAttempt, int questionCount);
}
