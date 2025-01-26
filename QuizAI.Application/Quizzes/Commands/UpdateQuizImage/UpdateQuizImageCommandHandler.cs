using MediatR;
using QuizAI.Application.Common;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Services;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Quizzes.Commands.UpdateQuizImage;

public class UpdateQuizImageCommandHandler : IRequestHandler<UpdateQuizImageCommand, NewQuizId>
{
    private readonly IRepository _repository;
    private readonly IQuizService _quizService;
    private readonly IImageService _imageService;

    public UpdateQuizImageCommandHandler(IRepository repository, IQuizService quizService, IImageService imageService)
    {
        _repository = repository;
        _quizService = quizService;
        _imageService = imageService;
    }

    public async Task<NewQuizId> Handle(UpdateQuizImageCommand request, CancellationToken cancellationToken)
    {
        var newQuiz = await _quizService.GetNewAndDeprecateOldAsync(request.GetId());

        await _repository.AddAsync(newQuiz);

        var newImage = await _imageService.UploadAsync(request.Image);
        newQuiz.ImageId = newImage.Id;

        await _repository.SaveChangesAsync();

        return new NewQuizId(newQuiz.Id);
    }
}
