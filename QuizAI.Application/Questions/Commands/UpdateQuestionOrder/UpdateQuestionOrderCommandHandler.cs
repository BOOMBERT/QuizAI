using MediatR;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Questions.Commands.UpdateQuestionOrder;

public class UpdateQuestionOrderCommandHandler : IRequestHandler<UpdateQuestionOrderCommand>
{
    private readonly IRepository _repository;
    private readonly IQuestionsRepository _questionsRepository;

    public UpdateQuestionOrderCommandHandler(IRepository repository, IQuestionsRepository questionsRepository)
    {
        _repository = repository;
        _questionsRepository = questionsRepository;
    }

    public async Task Handle(UpdateQuestionOrderCommand request, CancellationToken cancellationToken)
    {
        var quizId = request.GetQuizId();

        if (!await _repository.EntityExistsAsync<Quiz>(quizId))
            throw new NotFoundException($"Quiz with ID {quizId} was not found");

        var questions = await _questionsRepository.GetAllAsync(quizId);

        if (questions.Count == 0)
        {
            throw new NotFoundException($"Quiz with ID {quizId} has no associated questions.");
        }

        if (questions.Count != request.OrderChanges.Count)
        {
            throw new ConflictException($"The quiz contains {questions.Count} questions, but {request.OrderChanges.Count} order changes were provided.");
        }

        var questionsById = questions.ToDictionary(qn => qn.Id, qn => qn);

        foreach (var orderChange in request.OrderChanges)
        {
            if (!questionsById.ContainsKey(orderChange.QuestionId))
            {
                throw new NotFoundException($"Question with ID {orderChange.QuestionId} was not found in quiz with ID {quizId}.");
            }

            var question = questionsById[orderChange.QuestionId];
            question.Order = (byte)orderChange.NewOrder;
        }

        await _repository.SaveChangesAsync();
    }
}
