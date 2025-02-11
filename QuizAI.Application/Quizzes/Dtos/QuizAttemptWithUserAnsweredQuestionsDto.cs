using QuizAI.Application.Questions.Dtos;

namespace QuizAI.Application.Quizzes.Dtos;

public record QuizAttemptWithUserAnsweredQuestionsDto(
    IEnumerable<UserAnsweredQuestionDto> UserAnsweredQuestion, 
    int CorrectAnswerCount, 
    int QuestionCount,
    DateTime QuizStartedAt,
    DateTime QuizFinishedAt
    );
