using AutoMapper;
using QuizAI.Domain.Entities;

namespace QuizAI.Application.Questions.Dtos;

public class QuestionsProfile : Profile
{
    public QuestionsProfile()
    {
        CreateMap<MultipleChoiceAnswer, MultipleChoiceAnswerDto>();
        CreateMap<OpenEndedAnswer, OpenEndedAnswerDto>();
        CreateMap<TrueFalseAnswer, TrueFalseAnswerDto>();
    }
}
