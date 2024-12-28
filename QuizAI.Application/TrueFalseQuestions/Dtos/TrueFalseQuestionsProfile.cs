using AutoMapper;
using QuizAI.Application.TrueFalseQuestions.Commands.CreateTrueFalseQuestion;
using QuizAI.Domain.Entities;

namespace QuizAI.Application.TrueFalseQuestions.Dtos;

public class TrueFalseQuestionsProfile : Profile
{
    public TrueFalseQuestionsProfile()
    {
        CreateMap<CreateTrueFalseQuestionCommand, Question>();
    }
}
