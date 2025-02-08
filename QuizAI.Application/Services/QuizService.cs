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

    public async Task<Quiz> GetNewAndDeprecateOldAsync(Guid oldQuizId)
    {
        var oldQuiz = await _quizzesRepository.GetAsync(oldQuizId, true, true)
            ?? throw new NotFoundException($"Quiz with ID {oldQuizId} was not found");

        if (oldQuiz.IsDeprecated)
            throw new NotFoundException($"Quiz with ID {oldQuizId} was not found");

        var newQuiz = _mapper.Map<Quiz>(oldQuiz);

        await DeprecateAsync(oldQuiz, newQuiz.Id);

        return newQuiz;
    }

    public async Task<Quiz> GetNewWithCopiedQuestionsAndDeprecateOldAsync(Guid oldQuizId, int? questionId = null)
    {
        var oldQuiz = await _quizzesRepository.GetAsync(oldQuizId, true, true)
            ?? throw new NotFoundException($"Quiz with ID {oldQuizId} was not found");

        if (oldQuiz.IsDeprecated)
            throw new NotFoundException($"Quiz with ID {oldQuizId} was not found");

        if (questionId != null && !oldQuiz.Questions.Any(qn => qn.Id == questionId))
            throw new NotFoundException($"Question with ID {questionId} in quiz with ID {oldQuizId} was not found.");

        var newQuiz = new Quiz
        {
            Id = Guid.NewGuid(),
            Name = oldQuiz.Name,
            Description = oldQuiz.Description,
            QuestionCount = oldQuiz.QuestionCount,
            CreatorId = oldQuiz.CreatorId,
            ImageId = oldQuiz.ImageId,
            Categories = await _categoryService.GetOrCreateEntitiesAsync(oldQuiz.Categories.Select(c => c.Name)),
            Questions = _mapper.Map<ICollection<Question>>(oldQuiz.Questions)
        };

        await DeprecateAsync(oldQuiz, newQuiz.Id);

        return newQuiz;
    }

    private async Task DeprecateAsync(Quiz oldQuiz, Guid newQuizId)
    {
        await _quizzesRepository.UpdateLatestVersionIdAsync(oldQuiz.Id, newQuizId);
        
        oldQuiz.LatestVersionId = newQuizId;
        oldQuiz.Categories.Clear();
        oldQuiz.IsDeprecated = true;
    }
}
