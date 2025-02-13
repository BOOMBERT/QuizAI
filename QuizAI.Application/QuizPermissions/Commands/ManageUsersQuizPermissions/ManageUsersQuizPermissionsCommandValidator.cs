using FluentValidation;

namespace QuizAI.Application.QuizPermissions.Commands.ManageUsersQuizPermissions;

public class ManageUsersQuizPermissionsCommandValidator : AbstractValidator<ManageUsersQuizPermissionsCommand>
{
    public ManageUsersQuizPermissionsCommandValidator()
    {
        RuleFor(qp => qp.GetUserEmail())
            .EmailAddress();

        RuleFor(qp => qp.CanPlay)
            .Must(cp => cp == true).WithMessage("User must have 'CanPlay' permission if 'CanEdit' permission is granted")
            .When(qp => qp.CanEdit == true);
    }
}
