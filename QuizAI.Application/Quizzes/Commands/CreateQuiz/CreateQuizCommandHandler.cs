using AutoMapper;
using MediatR;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Quizzes.Commands.CreateQuiz;

public class CreateQuizCommandHandler : IRequestHandler<CreateQuizCommand>
{
    private readonly IMapper _mapper;
    private readonly IRepository _repository;
    private readonly ICategoriesRepository _categoriesRepository;

    public CreateQuizCommandHandler(IMapper mapper, IRepository repository, ICategoriesRepository categoriesRepository)
    {
        _mapper = mapper;
        _repository = repository;
        _categoriesRepository = categoriesRepository;
    }

    public async Task Handle(CreateQuizCommand request, CancellationToken cancellationToken)
    {
        var quiz = _mapper.Map<Quiz>(request);

        if (request.Categories.Count != 0)
        {
            var existingCategories = await _categoriesRepository.GetExistingCategoriesAsync(request.Categories);
            var existingCategoriesNames = existingCategories.Select(ec => ec.Name).ToHashSet();

            var newCategories = request.Categories
                .Where(name => !existingCategoriesNames.Contains(name))
                .Select(name => new Category { Name = name });

            quiz.Categories = existingCategories.Concat(newCategories).ToList();
        }

        await _repository.AddAsync(quiz);
        await _repository.SaveChangesAsync();
    }
}
