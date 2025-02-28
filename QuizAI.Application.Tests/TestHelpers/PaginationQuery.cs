using QuizAI.Application.Interfaces;
using QuizAI.Domain.Constants;

namespace QuizAI.Application.Tests.TestHelpers
{
    internal class PaginationQuery : IPaginationQuery
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string? SortBy { get; set; }
        public SortDirection? SortDirection { get; set; }
    }
}
