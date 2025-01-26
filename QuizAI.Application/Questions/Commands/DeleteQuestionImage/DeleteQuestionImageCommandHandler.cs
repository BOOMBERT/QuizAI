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

public class DeleteQuestionImageCommandHandler : IRequestHandler<DeleteQuestionImageCommand, NewQuizId>
{
    private readonly IRepository _repository;
    private readonly IQuizService _quizService;
    private readonly IQuestionService _questionService;

    public DeleteQuestionImageCommandHandler(IRepository repository, IQuizService quizService, IQuestionService questionService)
    {
        _repository = repository;
        _quizService = quizService;
        _questionService = questionService;
    }

    public async Task<NewQuizId> Handle(DeleteQuestionImageCommand request, CancellationToken cancellationToken)
    {
        var newQuiz = await _quizService.GetNewWithCopiedQuestionsAndDeprecateOldAsync(request.QuizId, request.QuestionId);

        if (await _repository.GetFieldAsync<Question, Guid?>(request.QuestionId, "ImageId") == null)
            throw new NotFoundException($"Question with ID {request.QuestionId} in quiz with ID {request.QuizId} has no associated image.");

        newQuiz.Questions.First(qn => qn.Id == request.QuestionId).ImageId = null;
       
        _questionService.ResetIds(newQuiz.Questions);

        await _repository.AddAsync(newQuiz);
        await _repository.SaveChangesAsync();

        return new NewQuizId(newQuiz.Id);
    }
}
