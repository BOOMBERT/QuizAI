namespace QuizAI.Application.OpenEndedQuestions.Dtos;

public record OpenEndedAnswersDto(IList<string> ValidContent, bool VerificationByAI, bool IgnoreCaseAndSpaces);
