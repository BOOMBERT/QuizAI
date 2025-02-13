using MediatR;
using QuizAI.Application.Common;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Quizzes.Dtos;
using QuizAI.Domain.Constants;

namespace QuizAI.Application.Quizzes.Queries.GetAllQuizzes;

public class GetAllQuizzesQuery : IRequest<PagedResponse<QuizDto>>, IPaginationQuery
{
    public string? SearchPhrase { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public string? SortBy { get; set; }
    public SortDirection? SortDirection { get; set; }
    public ICollection<string> FilterByCategories { get; set; } = new List<string>();
    public bool FilterByCreator { get; set; }
    public bool FilterBySharedQuizzes { get; set; }
}
