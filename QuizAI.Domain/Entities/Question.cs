﻿using QuizAI.Domain.Enums;

namespace QuizAI.Domain.Entities;

public class Question
{
    public int Id { get; set; }
    public string Content { get; set; }
    public QuestionType Type { get; set; }
    public byte Order { get; set; }

    public Guid? ImageId { get; set; }
    public Image? Image { get; set; }

    public Guid QuizId { get; set; }
    public Quiz Quiz { get; set; }

    public ICollection<MultipleChoiceAnswer> MultipleChoiceAnswers { get; set; } = new List<MultipleChoiceAnswer>();
    public OpenEndedAnswer OpenEndedAnswer { get; set; }
    public TrueFalseAnswer TrueFalseAnswer { get; set; }
    public ICollection<UserAnswer> UserAnswers { get; set; } = new List<UserAnswer>();
}
