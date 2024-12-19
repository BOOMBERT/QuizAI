using AutoMapper;
using QuizAI.Application.Quizzes.Commands.CreateQuiz;
using QuizAI.Application.Quizzes.Commands.UpdateQuiz;
using QuizAI.Domain.Entities;

namespace QuizAI.Application.Quizzes.Dtos;

public class QuizzesProfile : Profile
{
    public QuizzesProfile()
    {
        CreateMap<CreateQuizCommand, Quiz>()
            .ForMember(
                dest => dest.Categories, 
                opt => opt.Ignore()
            )
            .ForMember(
                dest => dest.Image,
                opt => opt.Ignore()
            );

        CreateMap<UpdateQuizCommand, Quiz>()
            .ForMember(
                dest => dest.Categories,
                opt => opt.Ignore()
            );

        CreateMap<Quiz, QuizDto>()
            .ForCtorParam(nameof(QuizDto.HasImage), opt => opt.MapFrom(src => src.ImageId != null))
            .ForCtorParam(nameof(QuizDto.Categories), opt => opt.MapFrom(src => src.Categories.Select(c => c.Name)));
    }
}
