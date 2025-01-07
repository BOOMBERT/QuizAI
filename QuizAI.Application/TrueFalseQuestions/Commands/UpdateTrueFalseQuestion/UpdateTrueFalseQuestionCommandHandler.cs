using MediatR;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Enums;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.TrueFalseQuestions.Commands.UpdateTrueFalseQuestion;

public class UpdateTrueFalseQuestionCommandHandler : IRequestHandler<UpdateTrueFalseQuestionCommand>
{
    private readonly IRepository _repository;
    private readonly IQuestionsRepository _questionsRepository;

    public UpdateTrueFalseQuestionCommandHandler(IRepository repository, IQuestionsRepository questionsRepository)
    {
        _repository = repository;
        _questionsRepository = questionsRepository;
    }

    public async Task Handle(UpdateTrueFalseQuestionCommand request, CancellationToken cancellationToken)
    {
        var (quizId, questionId) = (request.GetQuizId(), request.GetQuestionId());

        if (!await _repository.EntityExistsAsync<Quiz>(quizId))
            throw new NotFoundException($"Quiz with ID {quizId} was not found");

        var question = await _questionsRepository.GetWithAnswerAsync(quizId, questionId, QuestionType.TrueFalse)
            ?? throw new NotFoundException($"Quiz with ID {quizId} does not contain a Question with ID {questionId}.");

        if (question.Content != request.Content)
            question.Content = request.Content;

        var answer = question.TrueFalseAnswer;
        if (answer.IsCorrect != request.IsCorrect)
            answer.IsCorrect = request.IsCorrect;

        await _repository.SaveChangesAsync();
    }
}
