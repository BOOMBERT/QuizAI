using QuizAI.Application.MultipleChoiceQuestions.Dtos;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Enums;

namespace QuizAI.Application.Interfaces;

public interface IAnswerService
{
    public void RemoveUnusedMultipleChoiceAnswers(Question question, ICollection<MultipleChoiceAnswersDto> requestedNewAnswers);
    Task UpdateOrAddNewAnswersAsync(Question question, ICollection<MultipleChoiceAnswer> newAnswers);
    void ValidateUserAnswer(ICollection<string> userAnswer, QuestionType questionType);
    Task<bool> CheckUserAnswerAsync(ICollection<string> userAnswer, Question question);
}
