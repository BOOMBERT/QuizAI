using QuizAI.Application.MultipleChoiceQuestions.Dtos;
using QuizAI.Application.OpenEndedQuestions.Dtos;
using QuizAI.Application.TrueFalseQuestions.Dtos;
using QuizAI.Domain.Enums;

namespace QuizAI.Application.Questions.Dtos;

public record QuestionWithAnswersForGenerationDto(
    string QuestionContent,
    QuestionType QuestionType,
    IEnumerable<MultipleChoiceAnswerDto> MultipleChoiceAnswers,
    OpenEndedAnswersDto? OpenEndedAnswer,
    TrueFalseAnswerDto? TrueFalseAnswer
    );
