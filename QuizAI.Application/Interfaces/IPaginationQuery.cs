using QuizAI.Domain.Constants;

namespace QuizAI.Application.Interfaces;

public interface IPaginationQuery
{
    int PageNumber { get; set; }
    int PageSize { get; set; }
    string? SortBy { get; set; }
    SortDirection? SortDirection { get; set; }
}
