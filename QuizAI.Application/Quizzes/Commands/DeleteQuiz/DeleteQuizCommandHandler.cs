using MediatR;
using QuizAI.Application.Interfaces;
using QuizAI.Domain.Constants;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Quizzes.Commands.DeleteQuiz;

public class DeleteQuizCommandHandler : IRequestHandler<DeleteQuizCommand>
{
    private readonly IRepository _repository;
    private readonly IQuizAuthorizationService _quizAuthorizationService;
    private readonly IQuizzesRepository _quizzesRepository;
    private readonly IQuizAttemptsRepository _quizAttemptsRepository;
    private readonly ICategoryService _categoryService;
    private readonly IImageService _imageService;
    private readonly IQuizPermissionsRepository _quizPermissionsRepository;

    public DeleteQuizCommandHandler(
        IRepository repository, IQuizAuthorizationService quizAuthorizationService, IQuizzesRepository quizzesRepository, IQuizAttemptsRepository quizAttemptsRepository, 
        ICategoryService categoryService, IImageService imageService, IQuizPermissionsRepository quizPermissionsRepository)
    {
        _repository = repository;
        _quizAuthorizationService = quizAuthorizationService;
        _quizzesRepository = quizzesRepository;
        _quizAttemptsRepository = quizAttemptsRepository;
        _categoryService = categoryService;
        _imageService = imageService;
        _quizPermissionsRepository = quizPermissionsRepository;
    }

    public async Task Handle(DeleteQuizCommand request, CancellationToken cancellationToken)
    {
        var quiz = await _quizzesRepository.GetAsync(request.GetId(), true);
        
        if (quiz == null || quiz.IsDeprecated)
            throw new NotFoundException($"Quiz with ID {request.GetId()} was not found");

        await _quizAuthorizationService.AuthorizeAsync(quiz, null, ResourceOperation.Delete);

        await _categoryService.RemoveUnusedAsync(quiz, Enumerable.Empty<string>());

        if (!await _quizAttemptsRepository.HasAnyAsync(quiz.Id))
        {
            _repository.Remove(quiz);
            await _quizzesRepository.UpdateLatestVersionIdAsync(quiz.Id, null);

            if (quiz.ImageId != null)
                await _imageService.DeleteIfNotAssignedAsync((Guid)quiz.ImageId, quiz.Id);
        }
        else
        {
            quiz.IsDeprecated = true;
            await _quizPermissionsRepository.DeletePermissionsAsync(quiz.Id);
        }

        await _repository.SaveChangesAsync();
    }
}
