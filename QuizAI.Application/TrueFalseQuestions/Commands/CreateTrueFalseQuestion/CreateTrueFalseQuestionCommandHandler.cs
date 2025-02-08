using AutoMapper;
using MediatR;
using QuizAI.Application.Common;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Services;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Enums;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.TrueFalseQuestions.Commands.CreateTrueFalseQuestion;

public class CreateTrueFalseQuestionCommandHandler : IRequestHandler<CreateTrueFalseQuestionCommand, NewQuizId>
{
    private readonly IRepository _repository;
    private readonly IQuizService _quizService;
    private readonly IQuestionService _questionService;

    public CreateTrueFalseQuestionCommandHandler(IRepository repository, IQuizService quizService, IQuestionService questionService)
    {
        _repository = repository;
        _quizService = quizService;
        _questionService = questionService;
    }

    public async Task<NewQuizId> Handle(CreateTrueFalseQuestionCommand request, CancellationToken cancellationToken)
    {
        var newQuiz = await _quizService.GetNewWithCopiedQuestionsAndDeprecateOldAsync(request.GetQuizId());

        newQuiz.QuestionCount += 1;

        var orderOfQuestion = await _questionService.GetOrderForNewQuestionAsync(request.GetQuizId());

        var question = new Question
        {
            Content = request.Content,
            Type = QuestionType.TrueFalse,
            Order = orderOfQuestion,
            QuizId = newQuiz.Id
        };

        var trueFalseAnswer = new TrueFalseAnswer
        {
            IsCorrect = request.IsCorrect,
            Question = question
        };

        _questionService.ResetIds(newQuiz.Questions);

        await _repository.AddAsync(newQuiz);
        await _repository.AddAsync(trueFalseAnswer);
        await _repository.SaveChangesAsync();

        return new NewQuizId(newQuiz.Id);
    }
}
