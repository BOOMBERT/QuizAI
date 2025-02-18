using MediatR;
using QuizAI.Application.Common;
using QuizAI.Application.Interfaces;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Quizzes.Commands.ChangeQuizPrivacy;

public class ChangeQuizPrivacyCommandHandler : IRequestHandler<ChangeQuizPrivacyCommand, LatestQuizId>
{
    private readonly IRepository _repository;
    private readonly IQuizService _quizService;
    private readonly IImagesRepository _imagesRepository;
    private readonly IImageService _imageService;

    public ChangeQuizPrivacyCommandHandler(
        IRepository repository, IQuizService quizService, IImagesRepository imagesRepository, IImageService imageService)
    {
        _repository = repository;
        _quizService = quizService;
        _imagesRepository = imagesRepository;
        _imageService = imageService;
    }

    public async Task<LatestQuizId> Handle(ChangeQuizPrivacyCommand request, CancellationToken cancellationToken)
    {
        var (quizToUpdate, createdNewQuiz) = await _quizService.GetValidOrDeprecateAndCreateWithNewQuestionsAsync(request.GetId());

        if (quizToUpdate.IsPrivate == request.IsPrivate)
            throw new ConflictException($"The privacy setting of quiz with ID {request.GetId()} is already set to {request.IsPrivate}");

        quizToUpdate.IsPrivate = request.IsPrivate;

        if (createdNewQuiz)
            await _repository.AddAsync(quizToUpdate);

        await _repository.SaveChangesAsync();

        var allImages = await _imagesRepository.GetQuizAndItsQuestionImagesAsync(quizToUpdate.Id, quizToUpdate.ImageId);
        var imageIdsAndExtensions = new HashSet<(Guid, string)>();

        foreach (var image in allImages)
        {
            imageIdsAndExtensions.Add((image.Id, image.FileExtension));
        }

        if (allImages.Any())
            await _imageService.MoveImagesAsync(imageIdsAndExtensions, !quizToUpdate.IsPrivate, createdNewQuiz);

        return new LatestQuizId(quizToUpdate.Id);
    }
}
