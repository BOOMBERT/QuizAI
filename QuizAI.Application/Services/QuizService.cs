using QuizAI.Application.Interfaces;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Services;

public class QuizService : IQuizService
{
    private readonly IQuizzesRepository _quizzesRepository;

    public QuizService(IQuizzesRepository quizzesRepository)
    {
        _quizzesRepository = quizzesRepository;
    }

    public async Task DeprecateAsync(Quiz oldQuiz, Guid newQuizId)
    {
        await _quizzesRepository.UpdateLatestVersionIdAsync(oldQuiz.Id, newQuizId);
        
        oldQuiz.LatestVersionId = newQuizId;
        oldQuiz.Categories.Clear();
        oldQuiz.IsDeprecated = true;
    }
}
