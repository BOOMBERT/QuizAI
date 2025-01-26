using QuizAI.Application.MultipleChoiceQuestions.Dtos;
using QuizAI.Domain.Entities;

namespace QuizAI.Application.Interfaces;

public interface IQuestionService
{
    Task<byte> GetOrderForNewQuestionAsync(Guid quizId);
    void RemoveAndAdjustOrder(Quiz quiz, int questionToDeleteId);
    ICollection<MultipleChoiceAnswer> RemoveUnusedMultipleChoiceAnswersAndReturnNew(
        Question question, ICollection<CreateMultipleChoiceAnswerDto> requestedNewAnswers);
    Task UpdateOrAddNewAnswersAsync(Question question, ICollection<MultipleChoiceAnswer> newAnswers);
    void ResetIds(IEnumerable<Question> questions);
}
