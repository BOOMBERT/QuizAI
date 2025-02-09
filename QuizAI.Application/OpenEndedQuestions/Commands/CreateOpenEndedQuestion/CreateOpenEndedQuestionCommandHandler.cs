using AutoMapper;
using MediatR;
using QuizAI.Application.Common;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Services;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Enums;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.OpenEndedQuestions.Commands.CreateOpenEndedQuestion;

public class CreateOpenEndedQuestionCommandHandler : IRequestHandler<CreateOpenEndedQuestionCommand, LatestQuizId>
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

    public async Task<LatestQuizId> Handle(CreateOpenEndedQuestionCommand request, CancellationToken cancellationToken)
    {
        var (quiz, createdNewQuiz) = await _quizService.GetValidOrDeprecateAndCreateWithQuestionsAsync(request.GetQuizId());

        _questionService.ValidateQuestionLimit(quiz.QuestionCount);
        quiz.QuestionCount += 1;

        var question = new Question
        {
            Content = request.Content,
            Type = QuestionType.OpenEnded,
            Order = (byte)quiz.QuestionCount,
            QuizId = quiz.Id
        };

        var openEndedAnswer = new OpenEndedAnswer
        {
            ValidContent = request.Answers,
            VerificationByAI = request.VerificationByAI,
            Question = question
        };

        if (createdNewQuiz)
        {
            _questionService.ResetIds(quiz.Questions);
            await _repository.AddAsync(quiz);
        }

        await _repository.AddAsync(openEndedAnswer);
        await _repository.SaveChangesAsync();

        return new LatestQuizId(quiz.Id);
    }
}
