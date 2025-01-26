using AutoMapper;
using MediatR;
using QuizAI.Application.Common;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Services;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Enums;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.MultipleChoiceQuestions.Commands.UpdateMultipleChoiceQuestion;

public class UpdateMultipleChoiceQuestionCommandHandler : IRequestHandler<UpdateMultipleChoiceQuestionCommand, NewQuizId>
{
    private readonly IMapper _mapper;
    private readonly IRepository _repository;
    private readonly IQuizService _quizService;
    private readonly IQuestionService _questionService;

    public UpdateMultipleChoiceQuestionCommandHandler(IMapper mapper, IRepository repository, IQuizService quizService, IQuestionService questionService)
    {
        _mapper = mapper;
        _repository = repository;
        _quizService = quizService;
        _questionService = questionService;
    }
    public async Task<NewQuizId> Handle(UpdateMultipleChoiceQuestionCommand request, CancellationToken cancellationToken)
    {
        var newQuiz = await _quizService.GetNewWithCopiedQuestionsAndDeprecateOldAsync(request.GetQuizId(), request.GetQuestionId());

        var questionToUpdate = newQuiz.Questions.First(qn => qn.Id == request.GetQuestionId());

        if (questionToUpdate.Content != request.Content)
            questionToUpdate.Content = request.Content;

        var newAnswers = _mapper.Map<ICollection<MultipleChoiceAnswer>>(request.Answers);

        questionToUpdate.MultipleChoiceAnswers = newAnswers;

        _questionService.ResetIds(newQuiz.Questions);

        await _repository.AddAsync(newQuiz);
        await _repository.SaveChangesAsync();

        return new NewQuizId(newQuiz.Id);
    }
}
