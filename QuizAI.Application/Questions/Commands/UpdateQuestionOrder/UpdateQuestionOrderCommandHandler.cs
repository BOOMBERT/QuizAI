using MediatR;
using QuizAI.Application.Common;
using QuizAI.Application.Interfaces;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Questions.Commands.UpdateQuestionOrder;

public class UpdateQuestionOrderCommandHandler : IRequestHandler<UpdateQuestionOrderCommand, LatestQuizId>
{
    private readonly IRepository _repository;
    private readonly IQuizService _quizService;
    private readonly IQuestionService _questionService;

    public UpdateQuestionOrderCommandHandler(IRepository repository, IQuizService quizService, IQuestionService questionService)
    {
        _repository = repository;
        _quizService = quizService;
        _questionService = questionService;
    }

    public async Task<LatestQuizId> Handle(UpdateQuestionOrderCommand request, CancellationToken cancellationToken)
    {
        _questionService.ValidateQuestionLimit(request.OrderChanges.Max(oc => oc.NewOrder));

        var (quiz, createdNewQuiz) = await _quizService.GetValidOrDeprecateAndCreateWithQuestionsAsync(request.GetQuizId());

        if (quiz.QuestionCount == 0)
            throw new NotFoundException($"Quiz with ID {request.GetQuizId()} has no associated questions.");

        if (quiz.QuestionCount != request.OrderChanges.Count)
            throw new ConflictException($"The quiz contains {quiz.QuestionCount} questions, but {request.OrderChanges.Count} order changes were provided.");

        _questionService.ChangeOrders(quiz, request.OrderChanges);

        if (createdNewQuiz)
        {
            _questionService.ResetIds(quiz.Questions);
            await _repository.AddAsync(quiz);
        }

        await _repository.SaveChangesAsync();

        return new LatestQuizId(quiz.Id);
    }
}
