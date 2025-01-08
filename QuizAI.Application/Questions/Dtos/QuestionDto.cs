using QuizAI.Domain.Enums;

namespace QuizAI.Application.Questions.Dtos;

public record QuestionDto(int Id, string Content, QuestionType Type, bool HasImage, IEnumerable<string> MultipleChoiceAnswers);
