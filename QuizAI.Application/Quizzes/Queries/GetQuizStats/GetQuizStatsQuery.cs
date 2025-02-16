using MediatR;
using QuizAI.Application.Quizzes.Dtos;

namespace QuizAI.Application.Quizzes.Queries.GetQuizStats;

public class GetQuizStatsQuery(Guid quizId, bool includeDeprecatedVersions) : IRequest<QuizStatsDto>
{
    public Guid QuizId { get; } = quizId;
    public bool IncludeDeprecatedVersions { get; } = includeDeprecatedVersions;
}
