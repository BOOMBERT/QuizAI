using AutoMapper;
using QuizAI.Application.Quizzes.Commands.CreateQuiz;
using QuizAI.Application.Quizzes.Commands.UpdateQuiz;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Enums;

namespace QuizAI.Application.Quizzes.Dtos;

public class QuizzesProfile : Profile
{
    public QuizzesProfile()
    {
        CreateMap<CreateQuizCommand, Quiz>()
            .ForMember(dest => dest.Categories, opt => opt.Ignore())
            .ForMember(dest => dest.Image, opt => opt.Ignore());

        CreateMap<UpdateQuizCommand, Quiz>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Categories, opt => opt.Ignore());

        CreateMap<Quiz, QuizDto>()
            .ForCtorParam(nameof(QuizDto.HasImage), opt => opt.MapFrom(src => src.ImageId != null))
            .ForCtorParam(nameof(QuizDto.Categories), opt => opt.MapFrom(src => src.Categories.Select(c => c.Name)))
            .ForCtorParam(nameof(QuizDto.CanEdit), opt => opt.MapFrom(src => false));

        CreateMap<Quiz, Quiz>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .ForMember(dest => dest.CreationDate, opt => opt.Ignore())
            .ForMember(dest => dest.Questions, opt => opt.MapFrom((src, dest) => src.Questions.Select(qn =>
            {
                var newQuestion = new Question
                {
                    QuizId = dest.Id,
                    Content = qn.Content,
                    Type = qn.Type,
                    Order = qn.Order,
                    ImageId = qn.ImageId
                };

                switch (qn.Type)
                {
                    case QuestionType.MultipleChoice:
                        newQuestion.MultipleChoiceAnswers = qn.MultipleChoiceAnswers.Select(mca => new MultipleChoiceAnswer
                        {
                            Content = mca.Content,
                            IsCorrect = mca.IsCorrect
                        }).ToList();
                        break;

                    case QuestionType.TrueFalse:
                        newQuestion.TrueFalseAnswer = new TrueFalseAnswer
                        {
                            IsCorrect = qn.TrueFalseAnswer.IsCorrect
                        };
                        break;

                    case QuestionType.OpenEnded:
                        newQuestion.OpenEndedAnswer = new OpenEndedAnswer
                        {
                            ValidContent = qn.OpenEndedAnswer.ValidContent,
                            VerificationByAI = qn.OpenEndedAnswer.VerificationByAI
                        };
                        break;
                }

                return newQuestion;
            })));
    }
}
