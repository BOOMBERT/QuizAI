using MediatR;
using QuizAI.Application.Common;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Services;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Questions.Commands.UpdateQuestionImage;

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

        var newImage = await _imageService.UploadAsync(request.Image);

        var question = quiz.Questions.First(qn => qn.Id == request.GetQuestionId());
        var previousImageId = question.ImageId;

        question.ImageId = newImage.Id;

        if (createdNewQuiz)
        {
            _questionService.ResetIds(quiz.Questions);
            await _repository.AddAsync(quiz);
        }
        else
        {
            if (previousImageId != null)
                await _imageService.DeleteIfNotAssignedAsync((Guid)previousImageId, null, question.Id);
        }

        await _repository.SaveChangesAsync();

        return new LatestQuizId(quiz.Id);
    }
}
