using AutoMapper;
using MediatR;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Questions.Dtos;
using QuizAI.Application.Questions.Queries.GetQuestion;
using QuizAI.Application.Users;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Enums;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Questions.Queries.GetNextQuestion;

public class GetNextQuestionQueryHandler : IRequestHandler<GetNextQuestionQuery, QuestionDto>
{
    private readonly IMapper _mapper;
    private readonly IRepository _repository;
    private readonly IUserContext _userContext;
    private readonly IQuizzesRepository _quizzesRepository;
    private readonly IQuestionsRepository _questionsRepository;

    public GetNextQuestionQueryHandler(
        IMapper mapper, IRepository repository, IUserContext userContext, IQuizzesRepository quizzesRepository, IQuestionsRepository questionsRepository)
    {
        _mapper = mapper;
        _repository = repository;
        _userContext = userContext;
        _quizzesRepository = quizzesRepository;
        _questionsRepository = questionsRepository;
    }

    public async Task<QuestionDto> Handle(GetNextQuestionQuery request, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.GetCurrentUser();

        var quiz = await _quizzesRepository.GetAsync(request.QuizId) ?? 
            throw new NotFoundException($"Quiz with ID {request.QuizId} was not found");

        if (quiz.QuestionCount == 0)
            throw new NotFoundException($"No questions found for quiz with ID {request.QuizId}");
        
        var unfinishedQuizAttempt = await _quizzesRepository.GetUnfinishedAttemptAsync(request.QuizId, currentUser.Id);

        int currentOrder = 1;

        if (unfinishedQuizAttempt == null)
        {
            if (quiz.IsDeprecated)
                throw new NotFoundException($"Quiz with ID {request.QuizId} was not found");

            var quizAttempt = new QuizAttempt
            { 
                QuizId = request.QuizId, 
                UserId = currentUser.Id 
            };

            await _repository.AddAsync(quizAttempt);
            await _repository.SaveChangesAsync();
        }
        else
        {
            currentOrder = unfinishedQuizAttempt.CurrentOrder;
        }

        var question = await _questionsRepository.GetByOrderAsync(request.QuizId, currentOrder);

        return new QuestionDto(
            question!.Id,
            question.Content,
            question.Type,
            question.ImageId != null,
            question.Type == QuestionType.MultipleChoice 
                ? await _questionsRepository.GetMultipleChoiceAnswersContentAsync(question.Id)
                : Enumerable.Empty<string>()
            );
    }
}
