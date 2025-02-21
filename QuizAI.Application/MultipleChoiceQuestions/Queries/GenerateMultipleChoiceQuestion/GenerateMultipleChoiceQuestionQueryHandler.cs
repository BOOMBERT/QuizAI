﻿using MediatR;
using QuizAI.Application.Interfaces;
using QuizAI.Application.MultipleChoiceQuestions.Dtos;
using QuizAI.Domain.Constants;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.MultipleChoiceQuestions.Queries.GenerateMultipleChoiceQuestion;

public class GenerateMultipleChoiceQuestionQueryHandler : IRequestHandler<GenerateMultipleChoiceQuestionQuery, MultipleChoiceAnswersWithQuestionDto>
{
    private readonly IQuizzesRepository _quizzesRepository;
    private readonly IQuizAuthorizationService _quizAuthorizationService;
    private readonly IOpenAiService _openAiService;
    private readonly IQuestionService _questionService;

    public GenerateMultipleChoiceQuestionQueryHandler(
        IQuizzesRepository quizzesRepository, IQuizAuthorizationService quizAuthorizationService, IOpenAiService openAiService, IQuestionService questionService)
    {
        _quizzesRepository = quizzesRepository;
        _quizAuthorizationService = quizAuthorizationService;
        _openAiService = openAiService;
        _questionService = questionService;
    }

    public async Task<MultipleChoiceAnswersWithQuestionDto> Handle(GenerateMultipleChoiceQuestionQuery request, CancellationToken cancellationToken)
    {
        if (request.Suggestions?.Length > 255)
            throw new BadRequestException("Suggestions cannot exceed 255 characters");

        var quiz = await _quizzesRepository.GetAsync(request.QuizId, true, true, false)
            ?? throw new NotFoundException($"Quiz with ID {request.QuizId} was not found");

        await _quizAuthorizationService.AuthorizeAsync(quiz, null, ResourceOperation.RestrictedRead);

        if (quiz.QuestionCount < 2)
            throw new ConflictException($"Quiz with ID {quiz.Id} contains only {quiz.QuestionCount} question(s), but at least 2 are required to generate a new question");

        var quizQuestionsWithAnswers = quiz.Questions.Select(_questionService.MapToQuestionWithAnswersForGenerationDtoAsync);

        return await _openAiService.GenerateMultipleChoiceQuestionAsync(
            quiz.Name, quiz.Description, quiz.Categories.Select(c => c.Name), quizQuestionsWithAnswers, request.Suggestions);
    }
}
