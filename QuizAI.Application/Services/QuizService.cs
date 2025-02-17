using AutoMapper;
using QuizAI.Application.Interfaces;
using QuizAI.Domain.Constants;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Services;

public class QuizService : IQuizService
{
    private readonly IMapper _mapper;
    private readonly IRepository _repository;
    private readonly IQuizzesRepository _quizzesRepository;
    private readonly IQuizAuthorizationService _quizAuthorizationService;
    private readonly IQuizAttemptsRepository _quizAttemptsRepository;
    private readonly IQuizPermissionsRepository _quizPermissionsRepository;
    private readonly ICategoryService _categoryService;

    public QuizService(
        IMapper mapper, IRepository repository, IQuizzesRepository quizzesRepository, IQuizAuthorizationService quizAuthorizationService, 
        IQuizAttemptsRepository quizAttemptsRepository, IQuizPermissionsRepository quizPermissionsRepository, ICategoryService categoryService)
    {
        _mapper = mapper;
        _repository = repository;
        _quizzesRepository = quizzesRepository;
        _quizAuthorizationService = quizAuthorizationService;
        _quizAttemptsRepository = quizAttemptsRepository;
        _quizPermissionsRepository = quizPermissionsRepository;
        _categoryService = categoryService;
    }

    public async Task<(Quiz, bool)> GetValidOrDeprecateAndCreateAsync(Guid currentQuizId)
    {
        var currentQuiz = await _quizzesRepository.GetAsync(currentQuizId, true, true)
            ?? throw new NotFoundException($"Quiz with ID {currentQuizId} was not found");

        await _quizAuthorizationService.AuthorizeAsync(currentQuiz, null, ResourceOperation.Update);

        if (currentQuiz.IsDeprecated)
            throw new NotFoundQuizWithVersioningException(currentQuizId, currentQuiz.LatestVersionId);

        if (!await _quizAttemptsRepository.HasAnyAsync(currentQuiz.Id))
            return (currentQuiz, false);

        var newQuiz = _mapper.Map<Quiz>(currentQuiz);

        await DeprecateAsync(currentQuiz, newQuiz.Id);

        return (newQuiz, true);
    }

    public async Task<(Quiz, bool)> GetValidOrDeprecateAndCreateWithQuestionsAsync(Guid currentQuizId, int? questionId = null)
    {
        var currentQuiz = await _quizzesRepository.GetAsync(currentQuizId, true, true)
            ?? throw new NotFoundException($"Quiz with ID {currentQuizId} was not found");

        await _quizAuthorizationService.AuthorizeAsync(currentQuiz, null, ResourceOperation.Update);

        if (currentQuiz.IsDeprecated)
            throw new NotFoundQuizWithVersioningException(currentQuizId, currentQuiz.LatestVersionId);

        if (questionId != null && !currentQuiz.Questions.Any(qn => qn.Id == questionId))
            throw new NotFoundException($"Question with ID {questionId} in quiz with ID {currentQuizId} was not found");

        if (!await _quizAttemptsRepository.HasAnyAsync(currentQuiz.Id))
            return (currentQuiz, false);

        var newQuiz = new Quiz
        {
            Id = Guid.NewGuid(),
            Name = currentQuiz.Name,
            Description = currentQuiz.Description,
            QuestionCount = currentQuiz.QuestionCount,
            CreatorId = currentQuiz.CreatorId,
            ImageId = currentQuiz.ImageId,
            IsPrivate = currentQuiz.IsPrivate,
            Categories = await _categoryService.GetOrCreateEntitiesAsync(currentQuiz.Categories.Select(c => c.Name)),
            Questions = _mapper.Map<ICollection<Question>>(currentQuiz.Questions)
        };

        await DeprecateAsync(currentQuiz, newQuiz.Id);

        return (newQuiz, true);
    }

    private async Task DeprecateAsync(Quiz oldQuiz, Guid newQuizId)
    {
        await _quizzesRepository.UpdateLatestVersionIdAsync(oldQuiz.Id, newQuizId);

        var quizPermissions = await _quizPermissionsRepository.GetAllAsync(oldQuiz.Id);

        if (quizPermissions != null && quizPermissions.Any())
        {
            foreach (var quizPermission in quizPermissions)
            {
                if (await _quizAttemptsRepository.GetUnfinishedAsync(quizPermission.QuizId, quizPermission.UserId) == null)
                {
                    quizPermission.QuizId = newQuizId;
                }
                else
                {
                    var newQuizPermission = new QuizPermission
                    {
                        QuizId = newQuizId,
                        UserId = quizPermission.UserId,
                        CanEdit = quizPermission.CanEdit,
                        CanPlay = quizPermission.CanPlay
                    };

                    await _repository.AddAsync(newQuizPermission);
                }
            }
        }

        oldQuiz.ImageId = null;
        oldQuiz.LatestVersionId = newQuizId;
        oldQuiz.Categories.Clear();
        oldQuiz.IsDeprecated = true;
    }
}
