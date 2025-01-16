using AutoMapper;
using MediatR;
using QuizAI.Application.Interfaces;
using QuizAI.Application.MultipleChoiceQuestions.Dtos;
using QuizAI.Domain.Entities;
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

    public async Task<byte> GetOrderAsync(Guid quizId)
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

    public async Task DeleteAsync(Guid quizId, int questionId)
    {
        if (!await _repository.EntityExistsAsync<Quiz>(quizId))
            throw new NotFoundException($"Quiz with ID {quizId} was not found");

        var questions = await _questionsRepository.GetAllAsync(quizId);

        var questionToDelete = questions.FirstOrDefault(qn => qn.Id == questionId)
            ?? throw new NotFoundException($"Question with ID {questionId} was not found in quiz with ID {quizId}.");

        var questionToDeleteOrder = questionToDelete.Order;

        foreach (var question in questions.Where(qn => qn.Order > questionToDeleteOrder))
        {
            question.Order--;
        }

        _repository.Remove(questionToDelete);
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
}
