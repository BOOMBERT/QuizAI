using MediatR;
using QuizAI.Application.Common;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Services;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Questions.Commands.UpdateQuestionImage;

public class UpdateQuestionImageCommandHandler : IRequestHandler<UpdateQuestionImageCommand, NewQuizId>
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

    public async Task<NewQuizId> Handle(UpdateQuestionImageCommand request, CancellationToken cancellationToken)
    {
        var (quizId, questionId) = (request.GetQuizId(), request.GetQuestionId());
        var newQuiz = await _quizService.GetNewWithCopiedQuestionsAndDeprecateOldAsync(quizId, questionId);

        var newImage = await _imageService.UploadAsync(request.Image);
        newQuiz.Questions.First(qn => qn.Id == questionId).ImageId = newImage.Id;

        _questionService.ResetIds(newQuiz.Questions);

        await _repository.AddAsync(newQuiz);
        await _repository.SaveChangesAsync();

        return new NewQuizId(newQuiz.Id);
    }
}
