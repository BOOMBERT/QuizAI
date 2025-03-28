﻿using MediatR;
using QuizAI.Application.Common;
using QuizAI.Application.Questions.Dtos;

namespace QuizAI.Application.Questions.Commands.UpdateQuestionOrder;

public class UpdateQuestionOrderCommand : IRequest<LatestQuizId>
{
    private Guid QuizId { get; set; }
    public ICollection<UpdateQuestionOrderDto> OrderChanges { get; set; } = new List<UpdateQuestionOrderDto>();

    public void SetQuizId(Guid id)
    {
        QuizId = id;
    }

    public Guid GetQuizId()
    {
        return QuizId;
    }
}
