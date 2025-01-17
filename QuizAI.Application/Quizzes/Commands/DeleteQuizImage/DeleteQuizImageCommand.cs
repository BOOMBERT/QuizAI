﻿using MediatR;
using QuizAI.Application.Common;

namespace QuizAI.Application.Quizzes.Commands.DeleteQuizImage;

public class DeleteQuizImageCommand : IRequest<NewQuizId>
{
    private Guid Id { get; set; }

    public void SetId(Guid id)
    {
        Id = id;
    }

    public Guid GetId()
    {
        return Id;
    }
}
