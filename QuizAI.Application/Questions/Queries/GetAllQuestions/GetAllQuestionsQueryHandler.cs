using AutoMapper;
using MediatR;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Questions.Dtos;
using QuizAI.Application.Services;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Enums;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Questions.Queries.GetAllQuestions;

public class GetAllQuestionsQueryHandler : IRequestHandler<GetAllQuestionsQuery, IEnumerable<QuestionWithAnswerDto>>
{
    private readonly IRepository _repository;
    private readonly IQuestionsRepository _questionsRepository;
    private readonly IQuestionService _questionService;

    public GetAllQuestionsQueryHandler(IRepository repository, IQuestionsRepository questionsRepository, IQuestionService questionService)
    {
        _repository = repository;
        _questionsRepository = questionsRepository;
        _questionService = questionService;
    }

    public async Task<IEnumerable<QuestionWithAnswerDto>> Handle(GetAllQuestionsQuery request, CancellationToken cancellationToken)
    {
        if (!await _repository.EntityExistsAsync<Quiz>(request.QuizId))
            throw new NotFoundException($"Quiz with ID {request.QuizId} was not found");

        var questionsWithAnswers = await _questionsRepository.GetAllAsync(request.QuizId, true);

        return questionsWithAnswers.Select(_questionService.MapToQuestionWithAnswerDto);
    }
}
