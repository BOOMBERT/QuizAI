﻿using MediatR;
using QuizAI.Application.Common;
using QuizAI.Application.MultipleChoiceQuestions.Dtos;

namespace QuizAI.Application.MultipleChoiceQuestions.Commands.CreateMultipleChoiceQuestion;

public class CreateMultipleChoiceQuestionCommand : IRequest<LatestQuizId>
{
    private Guid QuizId { get; set; }
    public string Content { get; set; } = string.Empty;
    public ICollection<MultipleChoiceAnswersDto> Answers { get; set; } = new List<MultipleChoiceAnswersDto>();

    public void SetQuizId(Guid id)
    {
        QuizId = id;
    }

    public Guid GetQuizId()
    {
        return QuizId;
    }
}
