using QuizAI.Application.MultipleChoiceQuestions.Dtos;
using QuizAI.Application.Questions.Dtos;
using QuizAI.Domain.Entities;

namespace QuizAI.Application.Interfaces;

public interface IQuestionService
{
    public void RemoveUnusedMultipleChoiceAnswers(Question question, ICollection<CreateMultipleChoiceAnswerDto> requestedNewAnswers);
    Task UpdateOrAddNewAnswersAsync(Question question, ICollection<MultipleChoiceAnswer> newAnswers);
    void ValidateQuestionLimit(int questionCount);
    void RemoveAndAdjustOrder(Quiz quiz, Question questionToDelete);
    void ChangeOrders(Quiz quiz, ICollection<UpdateQuestionOrderDto> orderChanges);
    void ResetIds(IEnumerable<Question> questions);
    QuestionWithAnswerDto MapToQuestionWithAnswerDto(Question question);
}
