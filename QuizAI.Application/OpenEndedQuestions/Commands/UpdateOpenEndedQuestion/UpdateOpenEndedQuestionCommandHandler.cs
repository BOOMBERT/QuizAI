using MediatR;
using QuizAI.Application.Common;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Services;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Enums;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.OpenEndedQuestions.Commands.UpdateOpenEndedQuestion;

public class UpdateOpenEndedQuestionCommandHandler : IRequestHandler<UpdateOpenEndedQuestionCommand, NewQuizId>
{
    private readonly IRepository _repository;
    private readonly IQuizService _quizService;
    private readonly IQuestionService _questionService;

    public UpdateOpenEndedQuestionCommandHandler(IRepository repository, IQuizService quizService, IQuestionService questionService)
    {
        _repository = repository;
        _quizService = quizService;
        _questionService = questionService;
    }

    public async Task<NewQuizId> Handle(UpdateOpenEndedQuestionCommand request, CancellationToken cancellationToken)
    {
        var newQuiz = await _quizService.GetNewWithCopiedQuestionsAndDeprecateOldAsync(request.GetQuizId(), request.GetQuestionId());

        var questionToUpdate = newQuiz.Questions.First(qn => qn.Id == request.GetQuestionId());

        if (questionToUpdate.Content != request.Content)
            questionToUpdate.Content = request.Content;

        var answerOfQuestionToUpdate = questionToUpdate.OpenEndedAnswer;

        if (answerOfQuestionToUpdate.ValidContent != request.Answers)
            answerOfQuestionToUpdate.ValidContent = request.Answers;

        if (answerOfQuestionToUpdate.VerificationByAI != request.VerificationByAI)
            answerOfQuestionToUpdate.VerificationByAI = request.VerificationByAI;

        _questionService.ResetIds(newQuiz.Questions);

        await _repository.AddAsync(newQuiz);
        await _repository.SaveChangesAsync();

        return new NewQuizId(newQuiz.Id);
    }
}
