using AutoMapper;
using MediatR;
using QuizAI.Application.Common;
using QuizAI.Application.Interfaces;
using QuizAI.Application.MultipleChoiceQuestions.Dtos;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Enums;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Services;

public class QuestionService : IQuestionService
{
    private readonly IMapper _mapper;
    private readonly IRepository _repository;
    private readonly IQuestionsRepository _questionsRepository;
    private readonly byte _maxNumberOfQuestions;

    public QuestionService(IMapper mapper, IRepository repository, IQuestionsRepository questionsRepository, byte maxNumberOfQuestions)
    {
        _mapper = mapper;
        _repository = repository;
        _questionsRepository = questionsRepository;
        _maxNumberOfQuestions = maxNumberOfQuestions;
    }

    public async Task<byte> GetOrderForNewQuestionAsync(Guid quizId)
    {
        if (!await _repository.EntityExistsAsync<Quiz>(quizId))
            throw new NotFoundException($"Quiz with ID {quizId} was not found");

        var existingQuestionsCount = await _questionsRepository.HowManyAsync(quizId);
        if (existingQuestionsCount >= _maxNumberOfQuestions)
            throw new ConflictException(
                $"Quiz with ID {quizId} cannot have a new question " +
                $"because the number of questions would exceed the maximum limit of {_maxNumberOfQuestions}."
            );

        return (byte)(existingQuestionsCount + 1);
    }

    public void RemoveAndAdjustOrder(Quiz quiz, int questionToDeleteId)
    {
        var questionToDelete = quiz.Questions.First(qn => qn.Id == questionToDeleteId);
        
        quiz.Questions.Remove(questionToDelete);

        foreach (var question in quiz.Questions.Where(qn => qn.Order > questionToDelete.Order))
        {
            question.Order--;
        }
    }

    public ICollection<MultipleChoiceAnswer> RemoveUnusedMultipleChoiceAnswersAndReturnNew(
        Question question, ICollection<CreateMultipleChoiceAnswerDto> requestedNewAnswers)
    {
        var newAnswers = _mapper.Map<ICollection<MultipleChoiceAnswer>>(requestedNewAnswers);

        var answersToRemove = question.MultipleChoiceAnswers
            .Where(ea => !newAnswers.Any(na => na.Content == ea.Content));

        _repository.RemoveRange(answersToRemove);

        return newAnswers;
    }

    public async Task UpdateOrAddNewAnswersAsync(Question question, ICollection<MultipleChoiceAnswer> newAnswers)
    {
        foreach (var newAnswer in newAnswers)
        {
            var existingAnswer = question.MultipleChoiceAnswers.FirstOrDefault(ea => ea.Content == newAnswer.Content);
            if (existingAnswer != null)
            {
                if (existingAnswer.IsCorrect != newAnswer.IsCorrect)
                    existingAnswer.IsCorrect = newAnswer.IsCorrect;
            }
            else
            {
                newAnswer.Question = question;
                await _repository.AddAsync(newAnswer);
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
}
