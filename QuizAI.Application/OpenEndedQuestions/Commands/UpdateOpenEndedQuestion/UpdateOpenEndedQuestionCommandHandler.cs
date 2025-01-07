using MediatR;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Enums;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.OpenEndedQuestions.Commands.UpdateOpenEndedQuestion;

public class UpdateOpenEndedQuestionCommandHandler : IRequestHandler<UpdateOpenEndedQuestionCommand>
{
    private readonly IRepository _repository;
    private readonly IQuestionsRepository _questionsRepository;

    public UpdateOpenEndedQuestionCommandHandler(IRepository repository, IQuestionsRepository questionsRepository)
    {
        _repository = repository;
        _questionsRepository = questionsRepository;
    }

    public async Task Handle(UpdateOpenEndedQuestionCommand request, CancellationToken cancellationToken)
    {
        var (quizId, questionId) = (request.GetQuizId(), request.GetQuestionId());

        if (!await _repository.EntityExistsAsync<Quiz>(quizId))
            throw new NotFoundException($"Quiz with ID {quizId} was not found");

        var question = await _questionsRepository.GetWithAnswerAsync(quizId, questionId, QuestionType.OpenEnded)
            ?? throw new NotFoundException($"Quiz with ID {quizId} does not contain a Question with ID {questionId}.");

        if (question.Content != request.Content)
            question.Content = request.Content;

        var answer = question.OpenEndedAnswer;

        if (answer.ValidContent != request.Answers)
            answer.ValidContent = request.Answers;

        if (answer.VerificationByAI != request.VerificationByAI)
            answer.VerificationByAI = request.VerificationByAI;

        await _repository.SaveChangesAsync();
    }
}
