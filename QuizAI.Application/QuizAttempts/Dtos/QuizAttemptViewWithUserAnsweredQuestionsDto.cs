using QuizAI.Application.Questions.Dtos;

namespace QuizAI.Application.QuizAttempts.Dtos;

public record QuizAttemptViewWithUserAnsweredQuestionsDto(
    IEnumerable<UserAnsweredQuestionDto> UserAnsweredQuestions,
    QuizAttemptViewDto quizAttempt
    );
