using AutoMapper;
using MediatR;
using QuizAI.Application.Common;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Services;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Enums;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Questions.Commands.DeleteQuestionImage;

public class DeleteQuestionImageCommandHandler : IRequestHandler<DeleteQuestionImageCommand, LatestQuizId>
{
    private readonly IRepository _repository;
    private readonly IQuizService _quizService;
    private readonly IQuestionService _questionService;
    private readonly IImageService _imageService;

    public DeleteQuestionImageCommandHandler(IRepository repository, IQuizService quizService, IQuestionService questionService, IImageService imageService)
    {
        _repository = repository;
        _quizService = quizService;
        _questionService = questionService;
        _imageService = imageService;
    }

    public async Task<LatestQuizId> Handle(DeleteQuestionImageCommand request, CancellationToken cancellationToken)
    {
        var (quiz, createdNewQuiz) = await _quizService.GetValidOrDeprecateAndCreateWithQuestionsAsync(request.QuizId, request.QuestionId);

        var question = quiz.Questions.First(qn => qn.Id == request.QuestionId);
        var previousImageId = question.ImageId 
            ?? throw new NotFoundException($"Question with ID {request.QuestionId} in quiz with ID {request.QuizId} has no associated image.");
        
        question.ImageId = null;

        if (createdNewQuiz)
        {
            _questionService.ResetIds(quiz.Questions);
            await _repository.AddAsync(quiz);
        }
        else
        {
            await _imageService.DeleteIfNotAssignedAsync(previousImageId, null, request.QuestionId);
        }

        await _repository.SaveChangesAsync();

        return new LatestQuizId(quiz.Id);
    }
}
