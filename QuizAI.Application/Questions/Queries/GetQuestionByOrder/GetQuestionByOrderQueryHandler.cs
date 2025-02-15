using MediatR;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Questions.Dtos;
using QuizAI.Domain.Constants;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Questions.Queries.GetQuestionByOrder;

public class GetQuestionByOrderQueryHandler : IRequestHandler<GetQuestionByOrderQuery, QuestionWithAnswersDto>
{
    private readonly IRepository _repository;
    private readonly IQuizAuthorizationService _quizAuthorizationService;
    private readonly IQuestionsRepository _questionsRepository;
    private readonly IQuestionService _questionService;

    public GetQuestionByOrderQueryHandler(
        IRepository repository, IQuizAuthorizationService quizAuthorizationService, IQuestionsRepository questionsRepository, IQuestionService questionService)
    {
        _repository = repository;
        _quizAuthorizationService = quizAuthorizationService;
        _questionsRepository = questionsRepository;
        _questionService = questionService;
    }

    public async Task<QuestionWithAnswersDto> Handle(GetQuestionByOrderQuery request, CancellationToken cancellationToken)
    {
        if (request.Order < 1)
            throw new ConflictException($"Order number {request.Order} is invalid. It must be at least 1");

        _questionService.ValidateQuestionLimit(request.Order);

        var quiz = await _repository.GetEntityAsync<Quiz>(request.QuizId, false)
            ?? throw new NotFoundException($"Quiz with ID {request.QuizId} was not found");

        await _quizAuthorizationService.AuthorizeAsync(quiz, null, ResourceOperation.RestrictedRead);

        var question = await _questionsRepository.GetByOrderAsync(request.QuizId, request.Order, true)
            ?? throw new NotFoundException($"Question with order {request.Order} was not found in quiz with ID {request.QuizId}.");

        return _questionService.MapToQuestionWithAnswersDto(question);
    }
}
