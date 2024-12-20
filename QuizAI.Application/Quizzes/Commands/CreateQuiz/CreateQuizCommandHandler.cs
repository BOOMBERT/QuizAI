using AutoMapper;
using MediatR;
using QuizAI.Application.Interfaces;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Quizzes.Commands.CreateQuiz;

public class CreateQuizCommandHandler : IRequestHandler<CreateQuizCommand, Guid>
{
    private readonly IMapper _mapper;
    private readonly IRepository _repository;
    private readonly IImageService _imageService;
    private readonly ICategoryService _categoryService;

    public CreateQuizCommandHandler(IMapper mapper, IRepository repository, IImageService imageService, ICategoryService categoryService)
    {
        _mapper = mapper;
        _repository = repository;
        _imageService = imageService;
        _categoryService = categoryService;
    }

    public async Task<Guid> Handle(CreateQuizCommand request, CancellationToken cancellationToken)
    {
        var quiz = _mapper.Map<Quiz>(request);

        quiz.Categories = await _categoryService.GetOrCreateEntitiesAsync(request.Categories);

        if (request.Image != null)
        {
            var uploadedImage = await _imageService.UploadAsync(request.Image);
            quiz.Image = uploadedImage;
        }

        await _repository.AddAsync(quiz);
        await _repository.SaveChangesAsync();

        return quiz.Id;
    }
}
