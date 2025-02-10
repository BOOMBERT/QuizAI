using AutoMapper;
using MediatR;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Questions.Dtos;
using QuizAI.Application.Services;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Enums;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Questions.Queries.GetQuestionByOrder;

public class GetQuestionByOrderQueryHandler : IRequestHandler<GetQuestionByOrderQuery, QuestionWithAnswerDto>
{
    private readonly IRepository _repository;
    private readonly IQuestionsRepository _questionsRepository;
    private readonly IQuestionService _questionService;

    public GetQuestionByOrderQueryHandler(IRepository repository, IQuestionsRepository questionsRepository, IQuestionService questionService)
    {
        _repository = repository;
        _questionsRepository = questionsRepository;
        _questionService = questionService;
    }

    public async Task<QuestionWithAnswerDto> Handle(GetQuestionByOrderQuery request, CancellationToken cancellationToken)
    {
        if (request.Order < 1)
            throw new ConflictException($"Order number {request.Order} is invalid. It must be at least 1");

        _questionService.ValidateQuestionLimit(request.Order);

        if (!await _repository.EntityExistsAsync<Quiz>(request.QuizId))
            throw new NotFoundException($"Quiz with ID {request.QuizId} was not found");

        var question = await _questionsRepository.GetByOrderAsync(request.QuizId, request.Order, true)
            ?? throw new NotFoundException($"Question with order {request.Order} was not found in quiz with ID {request.QuizId}.");

        return _questionService.MapToQuestionWithAnswerDto(question);
    }
}
