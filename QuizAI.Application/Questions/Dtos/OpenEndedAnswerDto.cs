namespace QuizAI.Application.Questions.Dtos;

public record OpenEndedAnswerDto(int Id, IList<string> ValidContent, bool VerificationByAI);
