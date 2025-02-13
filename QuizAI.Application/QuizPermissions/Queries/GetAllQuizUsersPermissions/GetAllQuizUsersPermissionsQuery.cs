using MediatR;
using QuizAI.Application.QuizPermissions.Dtos;

namespace QuizAI.Application.QuizPermissions.Queries.GetAllQuizUsersPermissions;

public class GetAllQuizUsersPermissionsQuery(Guid quizId) : IRequest<IEnumerable<QuizUsersPermissionsDto>>
{
    public Guid QuizId { get; } = quizId;
}
