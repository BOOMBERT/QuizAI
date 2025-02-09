using AutoMapper;
using MediatR;
using QuizAI.Application.Common;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Services;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Enums;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.MultipleChoiceQuestions.Commands.CreateMultipleChoiceQuestion;

public class CreateMultipleChoiceQuestionCommandHandler : IRequestHandler<CreateMultipleChoiceQuestionCommand, LatestQuizId>
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

    public async Task<LatestQuizId> Handle(CreateMultipleChoiceQuestionCommand request, CancellationToken cancellationToken)
    {
        var (quiz, createdNewQuiz) = await _quizService.GetValidOrDeprecateAndCreateWithQuestionsAsync(request.GetQuizId());

        _questionService.ValidateQuestionLimit(quiz.QuestionCount);
        quiz.QuestionCount += 1;

        var question = new Question
        {
            Content = request.Content,
            Type = QuestionType.MultipleChoice,
            Order = (byte)quiz.QuestionCount,
            QuizId = quiz.Id
        };

        var multipleChoiceAnswers = _mapper.Map<IEnumerable<MultipleChoiceAnswer>>(request.Answers);

        foreach (var multipleChoiceAnswer in multipleChoiceAnswers)
        {
            multipleChoiceAnswer.Question = question;
        }

        if (createdNewQuiz)
        {
            _questionService.ResetIds(quiz.Questions);
            await _repository.AddAsync(quiz);
        }

        await _repository.AddRangeAsync(multipleChoiceAnswers);
        await _repository.SaveChangesAsync();

        return new LatestQuizId(quiz.Id);
    }
}
