using AutoMapper;
using MediatR;
using QuizAI.Application.Common;
using QuizAI.Application.Interfaces;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.MultipleChoiceQuestions.Commands.UpdateMultipleChoiceQuestion;

public class UpdateMultipleChoiceQuestionCommandHandler : IRequestHandler<UpdateMultipleChoiceQuestionCommand, LatestQuizId>
{
    private readonly IMapper _mapper;
    private readonly IRepository _repository;
    private readonly IQuizService _quizService;
    private readonly IQuestionService _questionService;
    private readonly IAnswerService _answerService;

    public UpdateMultipleChoiceQuestionCommandHandler(
        IMapper mapper, IRepository repository, IQuizService quizService, IQuestionService questionService, IAnswerService answerService)
    {
        _mapper = mapper;
        _repository = repository;
        _quizService = quizService;
        _questionService = questionService;
        _answerService = answerService;
    }
    public async Task<LatestQuizId> Handle(UpdateMultipleChoiceQuestionCommand request, CancellationToken cancellationToken)
    {
        var (quiz, createdNewQuiz) = await _quizService.GetValidOrDeprecateAndCreateWithQuestionsAsync(request.GetQuizId(), request.GetQuestionId());

        var questionToUpdate = quiz.Questions.First(qn => qn.Id == request.GetQuestionId());

        if (questionToUpdate.Content != request.Content)
            questionToUpdate.Content = request.Content;

        var newAnswers = _mapper.Map<ICollection<MultipleChoiceAnswer>>(request.Answers);

        if (createdNewQuiz)
        {
            questionToUpdate.MultipleChoiceAnswers = newAnswers;

            _questionService.ResetIds(quiz.Questions);
            await _repository.AddAsync(quiz);
        }
        else
        {
            _answerService.RemoveUnusedMultipleChoiceAnswers(questionToUpdate, request.Answers);
            await _answerService.UpdateOrAddNewAnswersAsync(questionToUpdate, newAnswers);
        }

        await _repository.SaveChangesAsync();

        return new LatestQuizId(quiz.Id);
    }
}
