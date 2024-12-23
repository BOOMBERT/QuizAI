using AutoMapper;
using MediatR;
using QuizAI.Application.Interfaces;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Quizzes.Commands.UpdateQuiz;

public class UpdateQuizCommandHandler : IRequestHandler<UpdateQuizCommand>
{
    private readonly IMapper _mapper;
    private readonly IRepository _repository;
    private readonly IQuizzesRepository _quizzesRepository;
    private readonly ICategoryService _categoryService;

    public UpdateQuizCommandHandler(IMapper mapper, IRepository repository, IQuizzesRepository quizzesRepository, ICategoryService categoryService)
    {
        _mapper = mapper;
        _repository = repository;
        _quizzesRepository = quizzesRepository;
        _categoryService = categoryService;
    }

    public async Task Handle(UpdateQuizCommand request, CancellationToken cancellationToken)
    {
        var quiz = await _quizzesRepository.GetWithCategoriesAsync(request.GetId())
            ?? throw new NotFoundException($"Quiz with ID {request.GetId()} was not found");

        await _categoryService.DeleteIfNotAssignedAsync(quiz, request.Categories);

        quiz.Categories = await _categoryService.GetOrCreateEntitiesAsync(request.Categories);

        _mapper.Map(request, quiz);
        await _repository.SaveChangesAsync();       
    }
}
