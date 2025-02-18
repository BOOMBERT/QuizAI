using MediatR;
using QuizAI.Application.Common;
using QuizAI.Application.Interfaces;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.QuestionImages.Commands.UpdateQuestionImage;

public class UpdateQuestionImageCommandHandler : IRequestHandler<UpdateQuestionImageCommand, LatestQuizId>
{
    private readonly IRepository _repository;
    private readonly IQuizService _quizService;
    private readonly IImageService _imageService;
    private readonly IQuestionService _questionService;

    public UpdateQuestionImageCommandHandler(IRepository repository, IQuizService quizService, IImageService imageService, IQuestionService questionService)
    {
        _repository = repository;
        _quizService = quizService;
        _imageService = imageService;
        _questionService = questionService;
    }

    public async Task<LatestQuizId> Handle(UpdateQuestionImageCommand request, CancellationToken cancellationToken)
    {
        var (quiz, createdNewQuiz) = await _quizService.GetValidOrDeprecateAndCreateWithQuestionsAsync(request.GetQuizId(), request.GetQuestionId());

        var newImage = await _imageService.UploadAsync(request.Image, quiz.IsPrivate);

        var question = quiz.Questions.First(qn => qn.Id == request.GetQuestionId());

        if (question.ImageId == newImage.Id)
            throw new ConflictException($"The image is already assigned to the question with ID {question.Id} in quiz with ID {quiz.Id}");

        if (createdNewQuiz)
        {
            _questionService.ResetIds(quiz.Questions);
            await _repository.AddAsync(quiz);
        }
        else
        {
            if (question.ImageId != null)
                await _imageService.DeleteIfNotAssignedAsync((Guid)question.ImageId, quiz.IsPrivate, null, question.Id);
        }

        question.ImageId = newImage.Id;

        await _repository.SaveChangesAsync();

        return new LatestQuizId(quiz.Id);
    }
}
