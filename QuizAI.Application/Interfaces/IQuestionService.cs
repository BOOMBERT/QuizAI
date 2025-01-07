using QuizAI.Application.MultipleChoiceQuestions.Dtos;
using QuizAI.Domain.Entities;

namespace QuizAI.Application.Interfaces;

public interface IQuestionService
{
    Task<byte> GetOrderAsync(Guid quizId);
    Task DeleteAsync(Guid quizId, int questionId);
    ICollection<MultipleChoiceAnswer> RemoveUnusedMultipleChoiceAnswersAndReturnNew(
        Question question, ICollection<CreateMultipleChoiceAnswerDto> requestedNewAnswers);
    Task UpdateOrAddNewAnswersAsync(Question question, ICollection<MultipleChoiceAnswer> newAnswers);
}
