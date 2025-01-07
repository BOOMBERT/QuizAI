using AutoMapper;
using MediatR;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Services;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Enums;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.MultipleChoiceQuestions.Commands.UpdateMultipleChoiceQuestion;

public class UpdateMultipleChoiceQuestionCommandHandler : IRequestHandler<UpdateMultipleChoiceQuestionCommand>
{
    private readonly IRepository _repository;
    private readonly IQuestionService _questionService;
    private readonly IQuestionsRepository _questionsRepository;

    public UpdateMultipleChoiceQuestionCommandHandler(IRepository repository, IQuestionService questionService, IQuestionsRepository questionsRepository)
    {
        _repository = repository;
        _questionService = questionService;
        _questionsRepository = questionsRepository;
    }
    public async Task Handle(UpdateMultipleChoiceQuestionCommand request, CancellationToken cancellationToken)
    {
        var (quizId, questionId) = (request.GetQuizId(), request.GetQuestionId());

        if (!await _repository.EntityExistsAsync<Quiz>(quizId))
            throw new NotFoundException($"Quiz with ID {quizId} was not found");

        var question = await _questionsRepository.GetWithAnswerAsync(quizId, questionId, QuestionType.MultipleChoice)
            ?? throw new NotFoundException($"Quiz with ID {quizId} does not contain a Question with ID {questionId}.");

        if (question.Content != request.Content)
            question.Content = request.Content;

        var newAnswers = _questionService.RemoveUnusedMultipleChoiceAnswersAndReturnNew(question, request.Answers);
        await _questionService.UpdateOrAddNewAnswersAsync(question, newAnswers);

        await _repository.SaveChangesAsync();
    }
}
