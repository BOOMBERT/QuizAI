using AutoMapper;
using QuizAI.Domain.Entities;

namespace QuizAI.Application.Questions.Dtos;

public class QuestionsProfile : Profile
{
    public QuestionsProfile()
    {
        CreateMap<Question, Question>()
            .ForMember(dest => dest.MultipleChoiceAnswers, opt => opt.MapFrom(src => src.MultipleChoiceAnswers.Select(mca => new MultipleChoiceAnswer
            {
                Id = mca.Id,
                Content = mca.Content,
                IsCorrect = mca.IsCorrect
            })))
            .ForMember(dest => dest.TrueFalseAnswer, opt => opt.MapFrom(src => src.TrueFalseAnswer == null ? null : new TrueFalseAnswer
            {
                Id = src.TrueFalseAnswer.Id,
                IsCorrect = src.TrueFalseAnswer.IsCorrect
            }))
            .ForMember(dest => dest.OpenEndedAnswer, opt => opt.MapFrom(src => src.OpenEndedAnswer == null ? null : new OpenEndedAnswer
            {
                Id = src.OpenEndedAnswer.Id,
                ValidContent = src.OpenEndedAnswer.ValidContent,
                VerificationByAI = src.OpenEndedAnswer.VerificationByAI,
                IgnoreCaseAndSpaces = src.OpenEndedAnswer.IgnoreCaseAndSpaces
            }));
    }
}
