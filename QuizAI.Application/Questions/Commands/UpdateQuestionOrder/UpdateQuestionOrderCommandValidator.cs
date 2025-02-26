﻿using FluentValidation;

namespace QuizAI.Application.Questions.Commands.UpdateQuestionOrder;

public class UpdateQuestionOrderCommandValidator : AbstractValidator<UpdateQuestionOrderCommand>
{
    public UpdateQuestionOrderCommandValidator()
    {
        RuleFor(uqo => uqo.OrderChanges)
            .Must(orderChanges => orderChanges.Min(oc => oc.NewOrder) >= 1).WithMessage("Each order number must be at least 1")
            .Must(orderChanges =>
            {
                var newOrders = orderChanges.Select(oc => oc.NewOrder).ToList();
                int minOrder = newOrders.Min();
                int maxOrder = newOrders.Max();
                int count = newOrders.Count;

                return minOrder == 1 && maxOrder == count && newOrders.Distinct().Count() == count;
            })
            .WithMessage("New orders must form a continuous sequence without gaps or duplicates")
            .Must(orderChanges => orderChanges.Select(oc => oc.QuestionId).Distinct().Count() == orderChanges.Count).WithMessage("Question IDs must be unique");
    }
}
