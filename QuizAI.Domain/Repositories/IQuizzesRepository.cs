namespace QuizAI.Domain.Repositories;

public interface IQuizzesRepository
{
    Task<Guid?> GetImageId(Guid quizId);
}
