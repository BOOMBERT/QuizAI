namespace QuizAI.Application.OpenEndedQuestions.Dtos;

public record OpenEndedAnswersWithQuestionDto(string QuestionContent, IList<string> ValidContent, bool VerificationByAI, bool IgnoreCaseAndSpaces);
