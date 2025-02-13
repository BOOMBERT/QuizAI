using MediatR;
using QuizAI.Application.Common;
using QuizAI.Application.Interfaces;
using QuizAI.Application.QuizAttempts.Dtos;
using QuizAI.Domain.Constants;

namespace QuizAI.Application.QuizAttempts.Queries.GetAllAttempts;

public class GetAllAttemptsQuery : IRequest<PagedResponse<QuizAttemptViewDto>>, IPaginationQuery
{
    public string? SearchPhrase { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public string? SortBy { get; set; }
    public SortDirection? SortDirection { get; set; }
    public Guid? FilterByQuizId { get; set; }
    public DateTime? FilterByStartedAtYearAndMonth { get; set; }
    public DateTime? FilterByFinishedAtYearAndMonth { get; set; }
}
