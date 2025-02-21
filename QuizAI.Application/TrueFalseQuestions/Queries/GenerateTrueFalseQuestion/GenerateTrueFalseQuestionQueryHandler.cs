using MediatR;
using QuizAI.Application.Interfaces;
using QuizAI.Application.TrueFalseQuestions.Dtos;
using QuizAI.Domain.Constants;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.TrueFalseQuestions.Queries.GenerateTrueFalseQuestion;

public class GenerateTrueFalseQuestionQueryHandler : IRequestHandler<GenerateTrueFalseQuestionQuery, TrueFalseAnswerWithQuestionDto>
{
    private readonly IQuizzesRepository _quizzesRepository;
    private readonly IQuizAuthorizationService _quizAuthorizationService;
    private readonly IOpenAiService _openAiService;
    private readonly IQuestionService _questionService;

    public GenerateTrueFalseQuestionQueryHandler(
        IQuizzesRepository quizzesRepository, IQuizAuthorizationService quizAuthorizationService, IOpenAiService openAiService, IQuestionService questionService)
    {
        _quizzesRepository = quizzesRepository;
        _quizAuthorizationService = quizAuthorizationService;
        _openAiService = openAiService;
        _questionService = questionService;
    }

    public async Task<TrueFalseAnswerWithQuestionDto> Handle(GenerateTrueFalseQuestionQuery request, CancellationToken cancellationToken)
    {
        var quiz = await _quizzesRepository.GetAsync(request.QuizId, true, true, false) 
            ?? throw new NotFoundException($"Quiz with ID {request.QuizId} was not found");
    
        await _quizAuthorizationService.AuthorizeAsync(quiz, null, ResourceOperation.RestrictedRead);

        if (quiz.QuestionCount < 2)
            throw new ConflictException($"Quiz with ID {quiz.Id} contains only {quiz.QuestionCount} question(s), but at least 2 are required to generate a new question");

        var quizQuestionsWithAnswers = await Task.WhenAll(
            quiz.Questions.Select(_questionService.MapToQuestionWithAnswersForGenerationDtoAsync)
            );

        return await _openAiService.GenerateTrueFalseQuestionAsync(
            quiz.Name, quiz.Description, quiz.Categories.Select(c => c.Name), quizQuestionsWithAnswers, request.Suggestions);
    }
}
