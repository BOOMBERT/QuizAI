using QuizAI.Domain.Entities;
using QuizAI.Domain.Enums;

namespace QuizAI.Application.Questions.Dtos;

public record QuestionWithAnswerDto(
    int Id, 
    string Content, 
    QuestionType Type, 
    int Order,
    bool HasImage,
    IEnumerable<MultipleChoiceAnswerDto> MultipleChoiceAnswers, 
    OpenEndedAnswerDto? OpenEndedAnswer, 
    TrueFalseAnswerDto? TrueFalseAnswer
    );
