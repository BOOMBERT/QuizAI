﻿using MediatR;
using QuizAI.Application.Questions.Dtos;

namespace QuizAI.Application.Questions.Queries.GetQuestionByOrder;

public class GetQuestionByOrderQuery(Guid quizId, int order) : IRequest<QuestionDto>
{
    public Guid QuizId { get; } = quizId;
    public int Order { get; } = order;
}
