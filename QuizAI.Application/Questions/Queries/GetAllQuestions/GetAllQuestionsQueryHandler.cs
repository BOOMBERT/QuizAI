using AutoMapper;
using MediatR;
using QuizAI.Application.Questions.Dtos;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Enums;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Questions.Queries.GetAllQuestions;

public class GetAllQuestionsQueryHandler : IRequestHandler<GetAllQuestionsQuery, IEnumerable<QuestionWithAnswerDto>>
{
    private readonly IMapper _mapper;
    private readonly IRepository _repository;
    private readonly IQuestionsRepository _questionsRepository;

    public GetAllQuestionsQueryHandler(IMapper mapper, IRepository repository, IQuestionsRepository questionsRepository)
    {
        _mapper = mapper;
        _repository = repository;
        _questionsRepository = questionsRepository;
    }

    public async Task<IEnumerable<QuestionWithAnswerDto>> Handle(GetAllQuestionsQuery request, CancellationToken cancellationToken)
    {
        if (!await _repository.EntityExistsAsync<Quiz>(request.QuizId))
            throw new NotFoundException($"Quiz with ID {request.QuizId} was not found");

        var questionsWithAnswers = await _questionsRepository.GetAsync(request.QuizId, true);
        var questionsWithAnswersToReturn = new List<QuestionWithAnswerDto>();

        foreach (var question in questionsWithAnswers)
        {
            var questionToAdd = new QuestionWithAnswerDto(
                question.Id,
                question.Content,
                question.Type,
                question.Order,
                question.ImageId != null,
                question.Type == QuestionType.MultipleChoice 
                    ? question.MultipleChoiceAnswers.Select(_mapper.Map<MultipleChoiceAnswerDto>) 
                    : Enumerable.Empty<MultipleChoiceAnswerDto>(),
                question.Type == QuestionType.OpenEnded 
                    ? _mapper.Map<OpenEndedAnswerDto>(question.OpenEndedAnswer) 
                    : null,
                question.Type == QuestionType.TrueFalse
                    ? _mapper.Map<TrueFalseAnswerDto>(question.TrueFalseAnswer) 
                    : null
                );

            questionsWithAnswersToReturn.Add(questionToAdd);
        }

        return questionsWithAnswersToReturn;
    }
}
