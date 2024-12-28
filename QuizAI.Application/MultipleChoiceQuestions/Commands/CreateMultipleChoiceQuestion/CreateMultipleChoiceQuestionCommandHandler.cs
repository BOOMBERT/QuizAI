using AutoMapper;
using MediatR;
using QuizAI.Application.Interfaces;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Enums;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.MultipleChoiceQuestions.Commands.CreateMultipleChoiceQuestion;

public class CreateMultipleChoiceQuestionCommandHandler : IRequestHandler<CreateMultipleChoiceQuestionCommand, int>
{
    private readonly IMapper _mapper;
    private readonly IRepository _repository;
    private readonly IQuestionService _questionService;

    public CreateMultipleChoiceQuestionCommandHandler(IMapper mapper, IRepository repository, IQuestionService questionService)
    {
        _mapper = mapper;
        _repository = repository;
        _questionService = questionService;
    }

    public async Task<int> Handle(CreateMultipleChoiceQuestionCommand request, CancellationToken cancellationToken)
    {
        var orderOfQuestion = await _questionService.GetOrderAsync(request.GetQuizId());

        var question = _mapper.Map<Question>(request);
        question.Type = QuestionType.MultipleChoice;
        question.Order = orderOfQuestion;

        var multipleChoiceAnswers = _mapper.Map<IEnumerable<MultipleChoiceAnswer>>(request.Answers);

        foreach (var multipleChoiceAnswer in multipleChoiceAnswers)
        {
            multipleChoiceAnswer.Question = question;
        }

        await _repository.AddRangeAsync(multipleChoiceAnswers);
        await _repository.SaveChangesAsync();

        return question.Id;
    }
}
