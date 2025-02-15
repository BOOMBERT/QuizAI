using MediatR;
using QuizAI.Application.Common;
using QuizAI.Application.Interfaces;
using QuizAI.Application.QuizAttempts.Dtos;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.QuizAttempts.Queries.GetAllAttempts;

public class GetAllAttemptsQueryHandler : IRequestHandler<GetAllAttemptsQuery, PagedResponse<QuizAttemptViewDto>>
{
    private readonly IUserContext _userContext;
    private readonly IQuizAttemptsRepository _quizAttemptsRepository;

    public GetAllAttemptsQueryHandler(IUserContext userContext, IQuizAttemptsRepository quizAttemptsRepository)
    {
        _userContext = userContext;
        _quizAttemptsRepository = quizAttemptsRepository;
    }

    public async Task<PagedResponse<QuizAttemptViewDto>> Handle(GetAllAttemptsQuery request, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.GetCurrentUser();

        var (quizAttempts, totalCount) = await _quizAttemptsRepository.GetAllMatchingFinishedAsync(
            currentUser.Id,
            request.SearchPhrase,
            request.PageSize,
            request.PageNumber,
            request.SortBy,
            request.SortDirection,
            request.FilterByQuizId,
            request.FilterByStartedAtYearAndMonth,
            request.FilterByFinishedAtYearAndMonth
            );

        var attemptsDtos = new List<QuizAttemptViewDto>();

        foreach (var attempt in quizAttempts)
        {
            attemptsDtos.Add(
                new QuizAttemptViewDto(
                    attempt.Id, 
                    attempt.QuizId, 
                    attempt.StartedAt, 
                    (DateTime)attempt.FinishedAt!, 
                    attempt.CorrectAnswers, 
                    attempt.Quiz.QuestionCount, 
                    attempt.Quiz.Name
                    )
                );
        }

        var paginationInfo = new PaginationInfo(totalCount, request.PageSize, request.PageNumber);
        
        return new PagedResponse<QuizAttemptViewDto>(attemptsDtos, paginationInfo);
    }
}
