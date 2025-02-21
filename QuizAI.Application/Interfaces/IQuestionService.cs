using QuizAI.Application.Questions.Dtos;
using QuizAI.Domain.Entities;

namespace QuizAI.Application.Interfaces;

public interface IQuestionService
{
    void ValidateQuestionLimit(int questionCount);
    void RemoveAndAdjustOrder(Quiz quiz, Question questionToDelete);
    void ChangeOrders(Quiz quiz, ICollection<UpdateQuestionOrderDto> orderChanges);
    void ResetIds(IEnumerable<Question> questions);
    Task<QuestionWithAnswersDto> MapToQuestionWithAnswersDtoAsync(Question question, bool isPrivate);
    Task<QuestionWithAnswersForGenerationDto> MapToQuestionWithAnswersForGenerationDtoAsync(Question question);
}
