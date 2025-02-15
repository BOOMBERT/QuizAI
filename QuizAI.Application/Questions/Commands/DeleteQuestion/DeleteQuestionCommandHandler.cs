using MediatR;
using QuizAI.Application.Common;
using QuizAI.Application.Interfaces;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Questions.Commands.DeleteQuestion;

public class DeleteQuestionCommandHandler : IRequestHandler<DeleteQuestionCommand, LatestQuizId>
{
    private readonly IRepository _repository;
    private readonly IQuizService _quizService;
    private readonly IQuestionService _questionService;
    private readonly IImageService _imageService;

    public DeleteQuestionCommandHandler(IRepository repository, IQuizService quizService, IQuestionService questionService, IImageService imageService)
    {
        _repository = repository;
        _quizService = quizService;
        _questionService = questionService;
        _imageService = imageService;
    }

    public async Task<LatestQuizId> Handle(DeleteQuestionCommand request, CancellationToken cancellationToken)
    {
        var (quiz, createdNewQuiz) = await _quizService.GetValidOrDeprecateAndCreateWithQuestionsAsync(request.QuizId, request.QuestionId);

        quiz.QuestionCount -= 1;

        var questionToDelete = quiz.Questions.First(qn => qn.Id == request.QuestionId);
        _questionService.RemoveAndAdjustOrder(quiz, questionToDelete);

        if (createdNewQuiz)
        {    
            _questionService.ResetIds(quiz.Questions);
            await _repository.AddAsync(quiz);
        }
        else
        {
            if (questionToDelete.ImageId != null)
                await _imageService.DeleteIfNotAssignedAsync((Guid)questionToDelete.ImageId, null, request.QuestionId);
        }

        await _repository.SaveChangesAsync();

        return new LatestQuizId(quiz.Id);
    }
}
