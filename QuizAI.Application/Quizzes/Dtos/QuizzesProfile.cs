using AutoMapper;
using QuizAI.Application.Quizzes.Commands.CreateQuiz;
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
            );
    }
}
