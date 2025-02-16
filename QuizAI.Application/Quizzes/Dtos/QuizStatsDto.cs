namespace QuizAI.Application.Quizzes.Dtos;

public record QuizStatsDto(int QuizAttemptsCount, double AverageCorrectAnswers, TimeSpan AverageTimeSpent);