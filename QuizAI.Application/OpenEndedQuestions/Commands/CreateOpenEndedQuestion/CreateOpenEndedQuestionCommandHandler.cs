using AutoMapper;
using MediatR;
using QuizAI.Application.Interfaces;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Enums;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.OpenEndedQuestions.Commands.CreateOpenEndedQuestion;

public class CreateOpenEndedQuestionCommandHandler : IRequestHandler<CreateOpenEndedQuestionCommand, int>
{
    private readonly IMapper _mapper;
    private readonly IRepository _repository;
    private readonly IQuestionService _questionService;

    public CreateOpenEndedQuestionCommandHandler(IMapper mapper, IRepository repository, IQuestionService questionService)
    {
        _mapper = mapper;
        _repository = repository;
        _questionService = questionService;
    }

    public async Task<int> Handle(CreateOpenEndedQuestionCommand request, CancellationToken cancellationToken)
    {
        var orderOfQuestion = await _questionService.GetOrderAsync(request.GetQuizId());

        var question = _mapper.Map<Question>(request);
        question.Type = QuestionType.OpenEnded;
        question.Order = orderOfQuestion;

        var openEndedAnswer = new OpenEndedAnswer
        {
            ValidContent = request.Answers,
            VerificationByAI = request.VerificationByAI,
            Question = question
        };

        await _repository.AddAsync(openEndedAnswer);
        await _repository.SaveChangesAsync();

        return question.Id;
    }
}
