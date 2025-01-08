using AutoMapper;
using MediatR;
using QuizAI.Application.Questions.Dtos;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Enums;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Questions.Queries.GetQuestionByOrder;

public class GetQuestionByOrderQueryHandler : IRequestHandler<GetQuestionByOrderQuery, QuestionDto>
{
    private readonly IMapper _mapper;
    private readonly IRepository _repository;
    private readonly IQuestionsRepository _questionsRepository;
    private const int MaxNumberOfQuestions = 20;

    public GetQuestionByOrderQueryHandler(IMapper mapper, IRepository repository, IQuestionsRepository questionsRepository)
    {
        _mapper = mapper;
        _repository = repository;
        _questionsRepository = questionsRepository;
    }

    public async Task<QuestionDto> Handle(GetQuestionByOrderQuery request, CancellationToken cancellationToken)
    {
        if (!await _repository.EntityExistsAsync<Quiz>(request.QuizId))
            throw new NotFoundException($"Quiz with ID {request.QuizId} was not found");

        if (request.Order < 1 || request.Order > MaxNumberOfQuestions)
            throw new ConflictException($"Order number must be between 1 and {MaxNumberOfQuestions}, inclusive.");

        var question = await _questionsRepository.GetByOrderAsync(request.QuizId, request.Order)
            ?? throw new NotFoundException($"Question with order {request.Order} was not found in quiz with ID {request.QuizId}.");

        return new QuestionDto(
            question.Id,
            question.Content,
            question.Type,
            question.ImageId != null,
            question.Type == QuestionType.MultipleChoice ? 
                await _questionsRepository.GetMultipleChoiceAnswersContentAsync(question.Id) 
                : Enumerable.Empty<string>()
            );
    }
}
