using MediatR;
using QuizAI.Application.Common;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Services;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Enums;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.TrueFalseQuestions.Commands.UpdateTrueFalseQuestion;

public class UpdateTrueFalseQuestionCommandHandler : IRequestHandler<UpdateTrueFalseQuestionCommand, NewQuizId>
{
    private readonly IRepository _repository;
    private readonly IQuizService _quizService;
    private readonly IQuestionService _questionService;

    public UpdateTrueFalseQuestionCommandHandler(IRepository repository, IQuizService quizService, IQuestionService questionService)
    {
        _repository = repository;
        _quizService = quizService;
        _questionService = questionService;
    }

    public async Task<NewQuizId> Handle(UpdateTrueFalseQuestionCommand request, CancellationToken cancellationToken)
    {
        var newQuiz = await _quizService.GetNewWithCopiedQuestionsAndDeprecateOldAsync(request.GetQuizId(), request.GetQuestionId());
        
        var questionToUpdate = newQuiz.Questions.First(qn => qn.Id == request.GetQuestionId());

        if (questionToUpdate.Content != request.Content)
            questionToUpdate.Content = request.Content;

        var answerOfQuestionToUpdate = questionToUpdate.TrueFalseAnswer;
        
        if (answerOfQuestionToUpdate.IsCorrect != request.IsCorrect)
            answerOfQuestionToUpdate.IsCorrect = request.IsCorrect;

        _questionService.ResetIds(newQuiz.Questions);

        await _repository.AddAsync(newQuiz);
        await _repository.SaveChangesAsync();

        return new NewQuizId(newQuiz.Id);
    }
}
