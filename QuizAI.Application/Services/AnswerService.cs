using QuizAI.Application.Interfaces;
using QuizAI.Application.MultipleChoiceQuestions.Dtos;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Enums;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Services;

public class AnswerService : IAnswerService
{
    private readonly IRepository _repository;
    private readonly IOpenAiService _openAiService;

    public AnswerService(IRepository repository, IOpenAiService openAiService)
    {
        _repository = repository;
        _openAiService = openAiService;
    }

    public void RemoveUnusedMultipleChoiceAnswers(Question question, ICollection<CreateMultipleChoiceAnswerDto> requestedNewAnswers)
    {
        var answersToRemove = question.MultipleChoiceAnswers
            .Where(mca => !requestedNewAnswers.Any(na => na.Content == mca.Content));

        _repository.RemoveRange(answersToRemove);
    }

    public async Task UpdateOrAddNewAnswersAsync(Question question, ICollection<MultipleChoiceAnswer> newAnswers)
    {
        foreach (var newAnswer in newAnswers)
        {
            var existingAnswer = question.MultipleChoiceAnswers.FirstOrDefault(mca => mca.Content == newAnswer.Content);
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

    public void ValidateUserAnswer(ICollection<string> userAnswer, QuestionType questionType)
    {
        if (questionType == QuestionType.TrueFalse)
        {
            var validAnswers = new HashSet<string> { "true", "false" };
            if (userAnswer.Count != 1 || !validAnswers.Contains(userAnswer.First()))
                throw new ConflictException(
                    "Invalid answer format for a true-false question - " +
                    "The answer must be either 'true' or 'false'");
        }
        else if (questionType == QuestionType.OpenEnded)
        {
            if (userAnswer.Count != 1 || userAnswer.First().Length > 1275)
                throw new ConflictException(
                    "Invalid answer format for an open-ended question - " +
                    "The answer must be a single response with a maximum length of 1275 characters");
        }
        else if (questionType == QuestionType.MultipleChoice)
        {
            if (userAnswer.Any(ua => ua.Length > 255))
                throw new ConflictException(
                    "Invalid answer format for a multiple-choice question - " +
                    "Each answer must not exceed 255 characters");
        }
    }

    public async Task<bool> CheckUserAnswerAsync(ICollection<string> userAnswer, Question question)
    {
        if (question.Type == QuestionType.TrueFalse)
        {
            return String.Equals(userAnswer.First(), question.TrueFalseAnswer.IsCorrect.ToString(), StringComparison.OrdinalIgnoreCase);
        }
        else if (question.Type == QuestionType.OpenEnded)
        {
            var answer = userAnswer.First();

            if (question.OpenEndedAnswer.IgnoreCaseAndSpaces)
            {
                if (question.OpenEndedAnswer.ValidContent.Any(vc => String.Equals(vc.Replace(" ", ""), answer.Replace(" ", ""), StringComparison.OrdinalIgnoreCase)))
                    return true;
            }
            else if (question.OpenEndedAnswer.ValidContent.Contains(answer))
            {
                return true;
            }

            if (question.OpenEndedAnswer.VerificationByAI)
                return await _openAiService.IsAnswerCorrectAsync(question.Content, answer, question.OpenEndedAnswer.ValidContent);

            return false;
        }
        else if (question.Type == QuestionType.MultipleChoice)
        {
            var correctAnswers = question.MultipleChoiceAnswers.Where(mca => mca.IsCorrect);
            if (correctAnswers.Count() != userAnswer.Count) return false;

            foreach (var correctAnswer in correctAnswers)
            {
                if (!userAnswer.Contains(correctAnswer.Content))
                    return false;
            }
            return true;
        }

        return false;
    }
}
