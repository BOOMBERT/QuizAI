using AutoMapper;
using QuizAI.Application.MultipleChoiceQuestions.Commands.CreateMultipleChoiceQuestion;
using QuizAI.Application.OpenEndedQuestions.Commands.CreateOpenEndedQuestion;
using QuizAI.Domain.Entities;

namespace QuizAI.Application.MultipleChoiceQuestions.Dtos;

public class MultipleChoiceQuestionsProfile : Profile
{
    public MultipleChoiceQuestionsProfile()
    {
        CreateMap<CreateMultipleChoiceAnswerDto, MultipleChoiceAnswer>();
    }
}
