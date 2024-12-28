using QuizAI.Application.Interfaces;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Services;

public class QuestionService : IQuestionService
{
    private readonly IRepository _repository;
    private readonly IQuizzesRepository _quizzesRepository;
    private readonly byte _maxNumberOfQuestions;

    public QuestionService(IRepository repository, IQuizzesRepository quizzesRepository, byte maxNumberOfQuestions)
    {
        _repository = repository;
        _quizzesRepository = quizzesRepository;
        _maxNumberOfQuestions = maxNumberOfQuestions;
    }

    public async Task<byte> GetOrderAsync(Guid quizId)
    {
        if (!await _repository.EntityExistsAsync<Quiz>(quizId))
            throw new NotFoundException($"Quiz with ID {quizId} was not found");

        var existingQuestionsCount = await _quizzesRepository.HowManyQuestions(quizId);
        if (existingQuestionsCount >= _maxNumberOfQuestions)
            throw new ConflictException(
                $"Quiz with ID {quizId} cannot have a new question " +
                $"because the number of questions would exceed the maximum limit of {_maxNumberOfQuestions}."
            );

        return (byte)(existingQuestionsCount + 1);
    }
}
