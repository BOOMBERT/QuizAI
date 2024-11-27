using AutoMapper;
using MediatR;
using QuizAI.Application.Interfaces;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Quizzes.Commands.CreateQuiz;

public class CreateQuizCommandHandler : IRequestHandler<CreateQuizCommand>
{
    private readonly IMapper _mapper;
    private readonly IRepository _repository;
    private readonly ICategoriesRepository _categoriesRepository;
    private readonly IImageService _imageServie;

    public CreateQuizCommandHandler(IMapper mapper, IRepository repository, ICategoriesRepository categoriesRepository, IImageService imageService)
    {
        _mapper = mapper;
        _repository = repository;
        _categoriesRepository = categoriesRepository;
        _imageServie = imageService;
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

        if (request.Image != null)
        {
            var uploadedImage = await _imageServie.UploadAsync(request.Image);
            quiz.Image = uploadedImage;
        }

        await _repository.AddAsync(quiz);
        await _repository.SaveChangesAsync();
    }
}
