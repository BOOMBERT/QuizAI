namespace QuizAI.Application.Questions.Dtos;

public record UserAnswerDto(Guid Id, IEnumerable<string> AnswerText, bool IsCorrect, DateTime AnsweredAt);
