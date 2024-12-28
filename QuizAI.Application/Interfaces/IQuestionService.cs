namespace QuizAI.Application.Interfaces;

public interface IQuestionService
{
    Task<byte> GetOrderAsync(Guid quizId);
}
