using AutoMapper;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Questions.Dtos;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Enums;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Services;

public class QuestionService : IQuestionService
{
    private readonly IMapper _mapper;
    private readonly IImagesRepository _imagesRepository;
    private readonly byte _maxNumberOfQuestions;

    public QuestionService(IMapper mapper, IImagesRepository imagesRepository, byte maxNumberOfQuestions)
    {
        _mapper = mapper;
        _imagesRepository = imagesRepository;
        _maxNumberOfQuestions = maxNumberOfQuestions;
    }

    public void ValidateQuestionLimit(int questionCount)
    {
        if (questionCount >= _maxNumberOfQuestions)
            throw new ConflictException(
                $"The question with order number {questionCount} cannot exist as it exceeds the maximum limit of {_maxNumberOfQuestions}"
            );
    }

    public void RemoveAndAdjustOrder(Quiz quiz, Question questionToDelete)
    {
        quiz.Questions.Remove(questionToDelete);

        foreach (var question in quiz.Questions.Where(qn => qn.Order > questionToDelete.Order))
        {
            question.Order--;
        }
    }

    public void ChangeOrders(Quiz quiz, ICollection<UpdateQuestionOrderDto> orderChanges)
    {
        var questionsById = quiz.Questions.ToDictionary(qn => qn.Id, qn => qn);

        foreach (var orderChange in orderChanges)
        {
            if (!questionsById.ContainsKey(orderChange.QuestionId))
            {
                throw new NotFoundException($"Question with ID {orderChange.QuestionId} was not found in quiz with ID {quiz.Id}");
            }

            var question = questionsById[orderChange.QuestionId];

            if (question.Order != (byte)orderChange.NewOrder)
            {
                question.Order = (byte)orderChange.NewOrder;
            }
        }
    }

    public void ResetIds(IEnumerable<Question> questions)
    {
        foreach (var question in questions)
        {
            question.Id = 0;

            switch (question.Type)
            {
                case QuestionType.MultipleChoice:
                    foreach (var multipleChoiceAnswer in question.MultipleChoiceAnswers)
                    {
                        multipleChoiceAnswer.Id = 0;
                    }
                    break;

                case QuestionType.OpenEnded:
                    question.OpenEndedAnswer.Id = 0;
                    break;

                case QuestionType.TrueFalse:
                    question.TrueFalseAnswer.Id = 0;
                    break;
            }
        }
    }

    public async Task<QuestionWithAnswersDto> MapToQuestionWithAnswersDtoAsync(Question question, bool isPrivate)
    {
        return new QuestionWithAnswersDto(
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
                   : null,
               (!isPrivate && question.ImageId != null) 
                   ? $"/api/uploads/{question.ImageId}{await _imagesRepository.GetFileExtensionAsync((Guid)question.ImageId)}"
                   : null
           );
    }
}
