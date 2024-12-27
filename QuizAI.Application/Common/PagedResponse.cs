namespace QuizAI.Application.Common;

public class PagedResponse<T>
{
    public IEnumerable<T> Data { get; set; }
    public PaginationInfo Pagination { get; set; }

    public PagedResponse(IEnumerable<T> data, PaginationInfo paginationInfo)
    {
        Data = data;
        Pagination = paginationInfo;
    }
}
