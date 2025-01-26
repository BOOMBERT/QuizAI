using AutoMapper;
using MediatR;
using QuizAI.Application.Interfaces;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Enums;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.TrueFalseQuestions.Commands.CreateTrueFalseQuestion;

public class CreateTrueFalseQuestionCommandHandler : IRequestHandler<CreateTrueFalseQuestionCommand, int>
{
    private readonly IMapper _mapper;
    private readonly IRepository _repository;
    private readonly IQuestionService _questionService;

    public CreateTrueFalseQuestionCommandHandler(IMapper mapper, IRepository repository, IQuestionService questionService)
    {
        _mapper = mapper;
        _repository = repository;
        _questionService = questionService;
    }

    public async Task<int> Handle(CreateTrueFalseQuestionCommand request, CancellationToken cancellationToken)
    {
        var orderOfQuestion = await _questionService.GetOrderForNewQuestionAsync(request.GetQuizId());

        var question = new Question
        {
            Content = request.Content,
            Type = QuestionType.TrueFalse,
            Order = orderOfQuestion,
            QuizId = request.GetQuizId()
        };

        var trueFalseAnswer = new TrueFalseAnswer
        {
            IsCorrect = request.IsCorrect,
            Question = question
        };

        await _repository.AddAsync(trueFalseAnswer);
        await _repository.SaveChangesAsync();

        return orderOfQuestion;
    }
}
