namespace QuizAI.Application.Interfaces;

public interface IOpenAiService
{
    Task<bool> IsAnswerCorrectAsync(string questionText, string answerText, IEnumerable<string> exampleCorrectAnswers);
}
