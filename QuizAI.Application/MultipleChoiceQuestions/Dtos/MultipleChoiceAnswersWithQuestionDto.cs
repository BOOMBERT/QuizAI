namespace QuizAI.Application.MultipleChoiceQuestions.Dtos;

public record MultipleChoiceAnswersWithQuestionDto(string QuestionContent, IEnumerable<MultipleChoiceAnswerDto> Answers);
