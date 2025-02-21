using AutoMapper;
using QuizAI.Domain.Entities;

namespace QuizAI.Application.MultipleChoiceQuestions.Dtos;

public class MultipleChoiceQuestionsProfile : Profile
{
    public MultipleChoiceQuestionsProfile()
    {
        CreateMap<MultipleChoiceAnswersDto, MultipleChoiceAnswer>();
    }
}
