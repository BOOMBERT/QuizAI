using MediatR;
using QuizAI.Application.Common;
using QuizAI.Application.Interfaces;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.TrueFalseQuestions.Commands.UpdateTrueFalseQuestion;

public class UpdateTrueFalseQuestionCommandHandler : IRequestHandler<UpdateTrueFalseQuestionCommand, LatestQuizId>
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

    public async Task<LatestQuizId> Handle(UpdateTrueFalseQuestionCommand request, CancellationToken cancellationToken)
    {
        var (quiz, createdNewQuiz) = await _quizService.GetValidOrDeprecateAndCreateWithQuestionsAsync(request.GetQuizId(), request.GetQuestionId());
        
        var questionToUpdate = quiz.Questions.First(qn => qn.Id == request.GetQuestionId());

        if (questionToUpdate.Content != request.Content)
            questionToUpdate.Content = request.Content;

        var answerOfQuestionToUpdate = questionToUpdate.TrueFalseAnswer;
        
        if (answerOfQuestionToUpdate.IsCorrect != request.IsCorrect)
            answerOfQuestionToUpdate.IsCorrect = request.IsCorrect;

        if (createdNewQuiz)
        {
            _questionService.ResetIds(quiz.Questions);
            await _repository.AddAsync(quiz);
        }

        await _repository.SaveChangesAsync();

        return new LatestQuizId(quiz.Id);
    }
}
