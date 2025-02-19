using MediatR;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Questions.Dtos;
using QuizAI.Domain.Constants;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Questions.Queries.GetAllQuestions;

public class GetAllQuestionsQueryHandler : IRequestHandler<GetAllQuestionsQuery, IEnumerable<QuestionWithAnswersDto>>
{
    private readonly IQuizzesRepository _quizzesRepository;
    private readonly IQuizAuthorizationService _quizAuthorizationService;
    private readonly IQuestionService _questionService;

    public GetAllQuestionsQueryHandler(IQuizzesRepository quizzesRepository, IQuizAuthorizationService quizAuthorizationService, IQuestionService questionService)
    {
        _quizzesRepository = quizzesRepository;
        _quizAuthorizationService = quizAuthorizationService;
        _questionService = questionService;
    }

    public async Task<IEnumerable<QuestionWithAnswersDto>> Handle(GetAllQuestionsQuery request, CancellationToken cancellationToken)
    {
        var quizWithQuestionsAndAnswers = await _quizzesRepository.GetAsync(request.QuizId, false, true, false) 
            ?? throw new NotFoundException($"Quiz with ID {request.QuizId} was not found");

        await _quizAuthorizationService.AuthorizeAsync(quizWithQuestionsAndAnswers, null, ResourceOperation.RestrictedRead);

        var questionWithAnswersDtos = new List<QuestionWithAnswersDto>();

        foreach (var question in quizWithQuestionsAndAnswers.Questions)
        {
            questionWithAnswersDtos.Add(await _questionService.MapToQuestionWithAnswersDtoAsync(question, quizWithQuestionsAndAnswers.IsPrivate));
        }

        return questionWithAnswersDtos;
    }
}
