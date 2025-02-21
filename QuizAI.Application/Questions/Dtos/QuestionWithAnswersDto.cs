using QuizAI.Application.MultipleChoiceQuestions.Dtos;
using QuizAI.Application.OpenEndedQuestions.Dtos;
using QuizAI.Application.TrueFalseQuestions.Dtos;
using QuizAI.Domain.Enums;

namespace QuizAI.Application.Questions.Dtos;

public record QuestionWithAnswersDto(
    int Id, 
    string Content, 
    QuestionType Type, 
    int Order,
    bool HasImage,
    IEnumerable<MultipleChoiceAnswerDto> MultipleChoiceAnswers, 
    OpenEndedAnswersDto? OpenEndedAnswer, 
    TrueFalseAnswerDto? TrueFalseAnswer,
    string? PublicImageUrl
    );
