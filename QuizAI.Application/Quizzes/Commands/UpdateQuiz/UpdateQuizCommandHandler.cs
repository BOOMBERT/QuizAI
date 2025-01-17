using AutoMapper;
using MediatR;
using QuizAI.Application.Common;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Services;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Enums;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Quizzes.Commands.UpdateQuiz;

public class UpdateQuizCommandHandler : IRequestHandler<UpdateQuizCommand, NewQuizId>
{
    private readonly IMapper _mapper;
    private readonly IRepository _repository;
    private readonly IQuizService _quizService;
    private readonly ICategoryService _categoryService;

    public UpdateQuizCommandHandler(IMapper mapper, IRepository repository, IQuizService quizService, ICategoryService categoryService)
    {
        _mapper = mapper;
        _repository = repository;
        _quizService = quizService;
        _categoryService = categoryService;
    }

    public async Task<NewQuizId> Handle(UpdateQuizCommand request, CancellationToken cancellationToken)
    {
        var newQuiz = await _quizService.GetNewAndDeprecateOldAsync(request.GetId());

        await _categoryService.RemoveUnusedAsync(newQuiz, request.Categories);

        newQuiz.Categories = await _categoryService.GetOrCreateEntitiesAsync(
            request.Categories.Except(newQuiz.Categories.Select(c => c.Name)));

        _mapper.Map(request, newQuiz);

        await _repository.AddAsync(newQuiz);
        await _repository.SaveChangesAsync();

        return new NewQuizId(newQuiz.Id);
    }
}
