using AutoMapper;
using MediatR;
using QuizAI.Application.Common;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Services;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Enums;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.OpenEndedQuestions.Commands.CreateOpenEndedQuestion;

public class CreateOpenEndedQuestionCommandHandler : IRequestHandler<CreateOpenEndedQuestionCommand, NewQuizId>
{
    private readonly IRepository _repository;
    private readonly IQuizService _quizService;
    private readonly IQuestionService _questionService;

    public CreateOpenEndedQuestionCommandHandler(IRepository repository, IQuizService quizService, IQuestionService questionService)
    {
        _repository = repository;
        _quizService = quizService;
        _questionService = questionService;
    }

    public async Task<NewQuizId> Handle(CreateOpenEndedQuestionCommand request, CancellationToken cancellationToken)
    {
        var newQuiz = await _quizService.GetNewWithCopiedQuestionsAndDeprecateOldAsync(request.GetQuizId());

        var orderOfQuestion = await _questionService.GetOrderForNewQuestionAsync(request.GetQuizId());

        var question = new Question
        {
            Content = request.Content,
            Type = QuestionType.OpenEnded,
            Order = orderOfQuestion,
            QuizId = newQuiz.Id
        };

        var openEndedAnswer = new OpenEndedAnswer
        {
            ValidContent = request.Answers,
            VerificationByAI = request.VerificationByAI,
            Question = question
        };

        _questionService.ResetIds(newQuiz.Questions);

        await _repository.AddAsync(newQuiz);
        await _repository.AddAsync(openEndedAnswer);
        await _repository.SaveChangesAsync();

        return new NewQuizId(newQuiz.Id);
    }
}
