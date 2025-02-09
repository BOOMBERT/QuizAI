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

public class UpdateQuizCommandHandler : IRequestHandler<UpdateQuizCommand, LatestQuizId>
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

    public async Task<LatestQuizId> Handle(UpdateQuizCommand request, CancellationToken cancellationToken)
    {
        var (quizToUpdate, createdNewQuiz) = await _quizService.GetValidOrDeprecateAndCreateAsync(request.GetId());

        await _categoryService.RemoveUnusedAsync(quizToUpdate, request.Categories);

        quizToUpdate.Categories = await _categoryService.GetOrCreateEntitiesAsync(
            request.Categories.Except(quizToUpdate.Categories.Select(c => c.Name)));

        _mapper.Map(request, quizToUpdate);

        if (createdNewQuiz)
            await _repository.AddAsync(quizToUpdate);
        
        await _repository.SaveChangesAsync();

        return new LatestQuizId(quizToUpdate.Id);
    }
}
