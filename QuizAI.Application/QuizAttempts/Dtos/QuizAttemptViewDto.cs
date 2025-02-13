using QuizAI.Domain.Entities;

namespace QuizAI.Application.QuizAttempts.Dtos;

public record QuizAttemptViewDto(Guid Id, Guid QuizId, DateTime StartedAt, DateTime FinishedAt, int CorrectAnswerCount, int QuestionCount, string QuizName);