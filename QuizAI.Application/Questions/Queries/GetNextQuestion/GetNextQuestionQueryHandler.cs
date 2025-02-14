using AutoMapper;
using MediatR;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Questions.Dtos;
using QuizAI.Application.Questions.Queries.GetQuestion;
using QuizAI.Application.Services;
using QuizAI.Application.Users;
using QuizAI.Domain.Constants;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Enums;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Questions.Queries.GetNextQuestion;

public class GetNextQuestionQueryHandler : IRequestHandler<GetNextQuestionQuery, NextQuestionDto>
{
    private readonly IRepository _repository;
    private readonly IUserContext _userContext;
    private readonly IQuizAuthorizationService _quizAuthorizationService;
    private readonly IQuestionsRepository _questionsRepository;
    private readonly IAnswersRepository _answersRepository;
    private readonly IQuizAttemptsRepository _quizAttemptsRepository;

    public GetNextQuestionQueryHandler(
        IRepository repository, IUserContext userContext, IQuizAuthorizationService quizAuthorizationService, 
        IQuestionsRepository questionsRepository, IAnswersRepository answersRepository, IQuizAttemptsRepository quizAttemptsRepository)
    {
        _repository = repository;
        _userContext = userContext;
        _quizAuthorizationService = quizAuthorizationService;
        _questionsRepository = questionsRepository;
        _answersRepository = answersRepository;
        _quizAttemptsRepository = quizAttemptsRepository;
    }

    public async Task<NextQuestionDto> Handle(GetNextQuestionQuery request, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.GetCurrentUser();

        var quiz = await _repository.GetEntityAsync<Quiz>(request.QuizId, false) ?? 
            throw new NotFoundException($"Quiz with ID {request.QuizId} was not found");

        await _quizAuthorizationService.AuthorizeAsync(quiz, currentUser.Id, ResourceOperation.Read);

        if (quiz.QuestionCount == 0)
            throw new NotFoundException($"No questions found for quiz with ID {request.QuizId}");
        
        var unfinishedQuizAttempt = await _quizAttemptsRepository.GetUnfinishedAsync(request.QuizId, currentUser.Id);

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

        return new NextQuestionDto(
            question!.Id,
            question.Content,
            question.Type,
            currentOrder,
            question.ImageId != null,
            question.Type == QuestionType.MultipleChoice 
                ? await _answersRepository.GetMultipleChoiceAnswersContentAsync(question.Id)
                : Enumerable.Empty<string>(),
            quiz.QuestionCount
            );
    }
}
