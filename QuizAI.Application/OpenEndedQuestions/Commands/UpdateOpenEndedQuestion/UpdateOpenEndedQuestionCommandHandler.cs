using MediatR;
using QuizAI.Application.Common;
using QuizAI.Application.Interfaces;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.OpenEndedQuestions.Commands.UpdateOpenEndedQuestion;

public class UpdateOpenEndedQuestionCommandHandler : IRequestHandler<UpdateOpenEndedQuestionCommand, LatestQuizId>
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

    public async Task<LatestQuizId> Handle(UpdateOpenEndedQuestionCommand request, CancellationToken cancellationToken)
    {
        var (quiz, createdNewQuiz) = await _quizService.GetValidOrDeprecateAndCreateWithQuestionsAsync(request.GetQuizId(), request.GetQuestionId());

        var questionToUpdate = quiz.Questions.First(qn => qn.Id == request.GetQuestionId());

        if (questionToUpdate.Content != request.Content)
            questionToUpdate.Content = request.Content;

        var answerOfQuestionToUpdate = questionToUpdate.OpenEndedAnswer;

        if (answerOfQuestionToUpdate.ValidContent != request.Answers)
            answerOfQuestionToUpdate.ValidContent = request.Answers;

        if (answerOfQuestionToUpdate.VerificationByAI != request.VerificationByAI)
            answerOfQuestionToUpdate.VerificationByAI = request.VerificationByAI;

        if (answerOfQuestionToUpdate.IgnoreCaseAndSpaces != request.IgnoreCaseAndSpaces)
            answerOfQuestionToUpdate.IgnoreCaseAndSpaces = request.IgnoreCaseAndSpaces;

        if (createdNewQuiz)
        {
            _questionService.ResetIds(quiz.Questions);
            await _repository.AddAsync(quiz);
        }

        await _repository.SaveChangesAsync();

        return new LatestQuizId(quiz.Id);
    }
}
