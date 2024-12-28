using AutoMapper;
using QuizAI.Application.OpenEndedQuestions.Commands.CreateOpenEndedQuestion;
using QuizAI.Domain.Entities;

namespace QuizAI.Application.OpenEndedQuestions.Dtos;

public class OpenEndedQuestionsProfile : Profile
{
    public OpenEndedQuestionsProfile()
    {
        CreateMap<CreateOpenEndedQuestionCommand, Question>();
    }
}
