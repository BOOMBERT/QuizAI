using MediatR;
using QuizAI.Application.Common;
using QuizAI.Application.Interfaces;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Enums;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.TrueFalseQuestions.Commands.CreateTrueFalseQuestion;

public class CreateTrueFalseQuestionCommandHandler : IRequestHandler<CreateTrueFalseQuestionCommand, LatestQuizId>
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

    public async Task<LatestQuizId> Handle(CreateTrueFalseQuestionCommand request, CancellationToken cancellationToken)
    {
        var (quiz, createdNewQuiz) = await _quizService.GetValidOrDeprecateAndCreateWithQuestionsAsync(request.GetQuizId());

        _questionService.ValidateQuestionLimit(quiz.QuestionCount);
        quiz.QuestionCount += 1;

        var question = new Question
        {
            Content = request.Content,
            Type = QuestionType.TrueFalse,
            Order = (byte)quiz.QuestionCount,
            QuizId = quiz.Id
        };

        var trueFalseAnswer = new TrueFalseAnswer
        {
            IsCorrect = request.IsCorrect,
            Question = question
        };

        if (createdNewQuiz)
        {
            _questionService.ResetIds(quiz.Questions);
            await _repository.AddAsync(quiz);
        }

        await _repository.AddAsync(trueFalseAnswer);
        await _repository.SaveChangesAsync();

        return new LatestQuizId(quiz.Id);
    }
}
