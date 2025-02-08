using AutoMapper;
using MediatR;
using QuizAI.Application.Common;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Services;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Enums;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.MultipleChoiceQuestions.Commands.CreateMultipleChoiceQuestion;

public class CreateMultipleChoiceQuestionCommandHandler : IRequestHandler<CreateMultipleChoiceQuestionCommand, NewQuizId>
{
    private readonly IMapper _mapper;
    private readonly IRepository _repository;
    private readonly IQuizService _quizService;
    private readonly IQuestionService _questionService;

    public CreateMultipleChoiceQuestionCommandHandler(IMapper mapper, IRepository repository, IQuizService quizService, IQuestionService questionService)
    {
        _mapper = mapper;
        _repository = repository;
        _quizService = quizService;
        _questionService = questionService;
    }

    public async Task<NewQuizId> Handle(CreateMultipleChoiceQuestionCommand request, CancellationToken cancellationToken)
    {
        var newQuiz = await _quizService.GetNewWithCopiedQuestionsAndDeprecateOldAsync(request.GetQuizId());

        newQuiz.QuestionCount += 1;

        var orderOfQuestion = await _questionService.GetOrderForNewQuestionAsync(request.GetQuizId());

        var question = new Question
        {
            Content = request.Content,
            Type = QuestionType.MultipleChoice,
            Order = orderOfQuestion,
            QuizId = newQuiz.Id
        };

        var multipleChoiceAnswers = _mapper.Map<IEnumerable<MultipleChoiceAnswer>>(request.Answers);

        foreach (var multipleChoiceAnswer in multipleChoiceAnswers)
        {
            multipleChoiceAnswer.Question = question;
        }

        _questionService.ResetIds(newQuiz.Questions);

        await _repository.AddAsync(newQuiz);
        await _repository.AddRangeAsync(multipleChoiceAnswers);
        await _repository.SaveChangesAsync();

        return new NewQuizId(newQuiz.Id);
    }
}
