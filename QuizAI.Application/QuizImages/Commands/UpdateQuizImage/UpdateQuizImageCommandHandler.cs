using MediatR;
using QuizAI.Application.Common;
using QuizAI.Application.Interfaces;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.QuizImages.Commands.UpdateQuizImage;

public class UpdateQuizImageCommandHandler : IRequestHandler<UpdateQuizImageCommand, LatestQuizId>
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

    public async Task<LatestQuizId> Handle(UpdateQuizImageCommand request, CancellationToken cancellationToken)
    {
        var (quiz, createdNewQuiz) = await _quizService.GetValidOrDeprecateAndCreateWithNewQuestionsAsync(request.GetId());
        
        var newImage = await _imageService.UploadAsync(request.Image, quiz.IsPrivate);

        if (quiz.ImageId == newImage.Id)
            throw new ConflictException($"The image is already assigned to the quiz with ID {quiz.Id}");

        if (createdNewQuiz)
            await _repository.AddAsync(quiz);
        
        if (quiz.ImageId != null)
            await _imageService.DeleteIfNotAssignedAsync((Guid)quiz.ImageId, quiz.IsPrivate, request.GetId());

        quiz.ImageId = newImage.Id;        

        await _repository.SaveChangesAsync();

        return new LatestQuizId(quiz.Id);
    }
}
