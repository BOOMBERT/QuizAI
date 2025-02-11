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

public class GetAllQuestionsQueryHandler : IRequestHandler<GetAllQuestionsQuery, IEnumerable<QuestionWithAnswersDto>>
{
    private readonly IQuestionsRepository _questionsRepository;
    private readonly IQuestionService _questionService;

    public GetAllQuestionsQueryHandler(IQuestionsRepository questionsRepository, IQuestionService questionService)
    {
        _questionsRepository = questionsRepository;
        _questionService = questionService;
    }

    public async Task<IEnumerable<QuestionWithAnswersDto>> Handle(GetAllQuestionsQuery request, CancellationToken cancellationToken)
    {
        var questionsWithAnswers = await _questionsRepository.GetAllAsync(request.QuizId, true) 
            ?? throw new NotFoundException($"Quiz with ID {request.QuizId} was not found"); ;

        return questionsWithAnswers.Select(_questionService.MapToQuestionWithAnswersDto);
    }
}
