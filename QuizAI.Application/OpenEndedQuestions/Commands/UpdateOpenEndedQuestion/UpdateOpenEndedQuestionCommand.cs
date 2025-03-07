﻿using MediatR;
using QuizAI.Application.Common;

namespace QuizAI.Application.OpenEndedQuestions.Commands.UpdateOpenEndedQuestion;

public class UpdateOpenEndedQuestionCommand : IRequest<LatestQuizId>
{
    private Guid QuizId { get; set; }
    private int QuestionId { get; set; }
    public string Content { get; set; } = string.Empty;
    public List<string> Answers { get; set; } = new List<string>();
    public bool VerificationByAI { get; set; }
    public bool IgnoreCaseAndSpaces { get; set; }

    public void SetQuizId(Guid id)
    {
        QuizId = id;
    }

    public Guid GetQuizId()
    {
        return QuizId;
    }

    public void SetQuestionId(int id)
    {
        QuestionId = id;
    }

    public int GetQuestionId()
    {
        return QuestionId;
    }
}
