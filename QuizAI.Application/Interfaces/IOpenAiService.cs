using QuizAI.Application.OpenEndedQuestions.Dtos;
using QuizAI.Application.Questions.Dtos;
using QuizAI.Application.TrueFalseQuestions.Dtos;

namespace QuizAI.Application.Interfaces;

public interface IOpenAiService
{
    Task<bool> IsAnswerCorrectAsync(string questionText, string answerText, IEnumerable<string> exampleCorrectAnswers);
    Task<TrueFalseAnswerWithQuestionDto> GenerateTrueFalseQuestionAsync(
        string quizName,
        string? quizDescription,
        IEnumerable<string> quizCategories,
        IEnumerable<QuestionWithAnswersForGenerationDto> quizQuestionsWithAnswers,
        string? userSuggestions);
    Task<OpenEndedAnswersWithQuestionDto> GenerateOpenEndedQuestionAsync(
        string quizName,
        string? quizDescription,
        IEnumerable<string> quizCategories,
        IEnumerable<QuestionWithAnswersForGenerationDto> quizQuestionsWithAnswers,
        string? userSuggestions);
}
