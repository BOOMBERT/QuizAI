using FluentValidation;

namespace QuizAI.Application.Questions.Commands.UpdateQuestionOrder;

public class UpdateQuestionOrderCommandValidator : AbstractValidator<UpdateQuestionOrderCommand>
{
    public UpdateQuestionOrderCommandValidator()
    {
        RuleFor(uqo => uqo.OrderChanges)
            .NotEmpty().WithMessage("Order changes cannot be empty")
            .Must(orderChanges => orderChanges.Count == 0 || orderChanges.Min(oc => oc.NewOrder) >= 1).WithMessage("Each order number must be at least 1")
            .Must(orderChanges =>
            {
                if (orderChanges.Count != 0)
                {
                    var newOrders = orderChanges.Select(oc => oc.NewOrder).ToList();
                    int minOrder = newOrders.Min();
                    int maxOrder = newOrders.Max();
                    int count = newOrders.Count;

                    return minOrder == 1 && maxOrder == count && newOrders.Distinct().Count() == count;
                }
                return true;
            }).WithMessage("New orders must form a continuous sequence without gaps or duplicates")
            .Must(orderChanges => orderChanges.Select(oc => oc.QuestionId).Distinct().Count() == orderChanges.Count).WithMessage("Question IDs must be unique");
    }
}
