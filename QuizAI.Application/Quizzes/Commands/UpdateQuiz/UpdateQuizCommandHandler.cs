using AutoMapper;
using MediatR;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Services;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Enums;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Quizzes.Commands.UpdateQuiz;

public class UpdateQuizCommandHandler : IRequestHandler<UpdateQuizCommand>
{
    private readonly IMapper _mapper;
    private readonly IRepository _repository;
    private readonly IQuizzesRepository _quizzesRepository;
    private readonly IQuizService _quizService;
    private readonly ICategoryService _categoryService;

    public UpdateQuizCommandHandler(
        IMapper mapper, IRepository repository, IQuizzesRepository quizzesRepository, IQuizService quizService, ICategoryService categoryService)
    {
        _mapper = mapper;
        _repository = repository;
        _quizzesRepository = quizzesRepository;
        _quizService = quizService;
        _categoryService = categoryService;
    }

    public async Task Handle(UpdateQuizCommand request, CancellationToken cancellationToken)
    {
        var oldQuiz = await _quizzesRepository.GetAsync(request.GetId(), true, true)
            ?? throw new NotFoundException($"Quiz with ID {request.GetId()} was not found");

        if (await _repository.GetFieldAsync<Quiz, bool>(request.GetId(), "IsDeprecated"))
            throw new ConflictException($"Quiz with ID {request.GetId()} is deprecated and cannot be updated");

        await _categoryService.RemoveUnusedAsync(oldQuiz, request.Categories);

        var newQuiz = _mapper.Map<Quiz>(oldQuiz);

        newQuiz.Categories = await _categoryService.GetOrCreateEntitiesAsync(
            request.Categories.Except(newQuiz.Categories.Select(c => c.Name)));

        _mapper.Map(request, newQuiz);

        await _quizService.DeprecateAsync(oldQuiz, newQuiz.Id);

        await _repository.AddAsync(newQuiz);
        await _repository.SaveChangesAsync();
    }
}
