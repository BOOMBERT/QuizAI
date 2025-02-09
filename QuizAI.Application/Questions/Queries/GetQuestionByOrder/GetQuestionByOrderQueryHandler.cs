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
    private readonly IMapper _mapper;
    private readonly IRepository _repository;
    private readonly IQuestionsRepository _questionsRepository;
    private readonly IQuestionService _questionService;
    private const int MaxNumberOfQuestions = 20;

    public GetQuestionByOrderQueryHandler(IMapper mapper, IRepository repository, IQuestionsRepository questionsRepository, IQuestionService questionService)
    {
        _mapper = mapper;
        _repository = repository;
        _questionsRepository = questionsRepository;
        _questionService = questionService;
    }

    public async Task<QuestionWithAnswerDto> Handle(GetQuestionByOrderQuery request, CancellationToken cancellationToken)
    {
        if (!await _repository.EntityExistsAsync<Quiz>(request.QuizId))
            throw new NotFoundException($"Quiz with ID {request.QuizId} was not found");

        if (request.Order < 1 || request.Order > MaxNumberOfQuestions)
            throw new ConflictException($"Order number must be between 1 and {MaxNumberOfQuestions}, inclusive.");

        var question = await _questionsRepository.GetByOrderAsync(request.QuizId, request.Order, true)
            ?? throw new NotFoundException($"Question with order {request.Order} was not found in quiz with ID {request.QuizId}.");

        return _questionService.MapToQuestionWithAnswerDto(question);
    }
}
