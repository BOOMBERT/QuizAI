using QuizAI.Domain.Enums;

namespace QuizAI.Application.Questions.Dtos;

public record NextQuestionDto(
    int Id, 
    string Content, 
    QuestionType Type, 
    int Order, 
    bool HasImage, 
    IEnumerable<string> MultipleChoiceAnswers, 
    int QuestionCount,
    string? PublicImageUrl
    );
