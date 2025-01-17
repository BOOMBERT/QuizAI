using MediatR;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Services;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Quizzes.Commands.DeleteQuiz;

public class DeleteQuizCommandHandler : IRequestHandler<DeleteQuizCommand>
{
    private readonly IRepository _repository;
    private readonly IQuizzesRepository _quizzesRepository;
    private readonly ICategoryService _categoryService;

    public DeleteQuizCommandHandler(IRepository repository, IQuizzesRepository quizzesRepository, ICategoryService categoryService)
    {
        _repository = repository;
        _quizzesRepository = quizzesRepository;
        _categoryService = categoryService;
    }

    public async Task Handle(DeleteQuizCommand request, CancellationToken cancellationToken)
    {
        var quiz = await _quizzesRepository.GetAsync(request.GetId(), true) 
            ?? throw new NotFoundException($"Quiz with ID {request.GetId()} was not found");
        
        if (quiz.IsDeprecated)
            throw new NotFoundException($"Quiz with ID {request.GetId()} was not found");

        await _categoryService.RemoveUnusedAsync(quiz, Enumerable.Empty<string>());
        quiz.IsDeprecated = true;

        await _repository.SaveChangesAsync();
    }
}
