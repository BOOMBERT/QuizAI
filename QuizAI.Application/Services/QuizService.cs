using AutoMapper;
using MediatR;
using QuizAI.Application.Interfaces;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Services;

public class QuizService : IQuizService
{
    private readonly IMapper _mapper;
    private readonly IQuizzesRepository _quizzesRepository;
    private readonly ICategoryService _categoryService;

    public QuizService(IMapper mapper, IQuizzesRepository quizzesRepository, ICategoryService categoryService)
    {
        _mapper = mapper;
        _quizzesRepository = quizzesRepository;
        _categoryService = categoryService;
    }

    public async Task<(Quiz, bool)> GetValidOrDeprecateAndCreateAsync(Guid currentQuizId)
    {
        var currentQuiz = await _quizzesRepository.GetAsync(currentQuizId, true, true)
            ?? throw new NotFoundException($"Quiz with ID {currentQuizId} was not found");

        if (currentQuiz.IsDeprecated)
            throw new NotFoundException($"Quiz with ID {currentQuizId} was not found");

        if (!await _quizzesRepository.HasAnyAttemptsAsync(currentQuiz.Id))
            return (currentQuiz, false);

        var newQuiz = _mapper.Map<Quiz>(currentQuiz);

        await DeprecateAsync(currentQuiz, newQuiz.Id);

        return (newQuiz, true);
    }

    public async Task<(Quiz, bool)> GetValidOrDeprecateAndCreateWithQuestionsAsync(Guid currentQuizId, int? questionId = null)
    {
        var currentQuiz = await _quizzesRepository.GetAsync(currentQuizId, true, true)
            ?? throw new NotFoundException($"Quiz with ID {currentQuizId} was not found");

        if (currentQuiz.IsDeprecated)
            throw new NotFoundException($"Quiz with ID {currentQuizId} was not found");

        if (questionId != null && !currentQuiz.Questions.Any(qn => qn.Id == questionId))
            throw new NotFoundException($"Question with ID {questionId} in quiz with ID {currentQuizId} was not found");

        if (!await _quizzesRepository.HasAnyAttemptsAsync(currentQuiz.Id))
            return (currentQuiz, false);

        var newQuiz = new Quiz
        {
            Id = Guid.NewGuid(),
            Name = currentQuiz.Name,
            Description = currentQuiz.Description,
            QuestionCount = currentQuiz.QuestionCount,
            CreatorId = currentQuiz.CreatorId,
            ImageId = currentQuiz.ImageId,
            Categories = await _categoryService.GetOrCreateEntitiesAsync(currentQuiz.Categories.Select(c => c.Name)),
            Questions = _mapper.Map<ICollection<Question>>(currentQuiz.Questions)
        };

        await DeprecateAsync(currentQuiz, newQuiz.Id);

        return (newQuiz, true);
    }

    private async Task DeprecateAsync(Quiz oldQuiz, Guid newQuizId)
    {
        await _quizzesRepository.UpdateLatestVersionIdAsync(oldQuiz.Id, newQuizId);

        oldQuiz.LatestVersionId = newQuizId;
        oldQuiz.Categories.Clear();
        oldQuiz.IsDeprecated = true;
    }
}
