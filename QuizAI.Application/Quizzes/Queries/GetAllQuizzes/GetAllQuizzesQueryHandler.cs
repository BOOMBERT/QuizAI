using AutoMapper;
using MediatR;
using QuizAI.Application.Common;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Quizzes.Dtos;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.Quizzes.Queries.GetAllQuizzes;

public class GetAllQuizzesQueryHandler : IRequestHandler<GetAllQuizzesQuery, PagedResponse<QuizDto>>
{
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;
    private readonly IQuizzesRepository _quizzesRepository;
    private readonly IQuizAuthorizationService _quizAuthorizationService;

    public GetAllQuizzesQueryHandler(IMapper mapper, IUserContext userContext, IQuizzesRepository quizzesRepository, IQuizAuthorizationService quizAuthorizationService)
    {
        _mapper = mapper;
        _userContext = userContext;
        _quizzesRepository = quizzesRepository;
        _quizAuthorizationService = quizAuthorizationService;
    }

    public async Task<PagedResponse<QuizDto>> Handle(GetAllQuizzesQuery request, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.GetCurrentUser();

        var (quizzes, totalCount) = await _quizzesRepository.GetAllMatchingAsync(
            currentUser.Id,
            request.SearchPhrase,
            request.PageSize,
            request.PageNumber,
            request.SortBy,
            request.SortDirection,
            request.FilterByCreator,
            request.FilterByCategories,
            request.FilterBySharedQuizzes
        );

        var quizzesDtos = _mapper.Map<IEnumerable<QuizDto>>(quizzes);

        if (request.FilterByCreator)
        {
            quizzesDtos = quizzesDtos.Select(dto => dto with { CanEdit = true });
        }
        else
        {
            var dtosWithUpdatedCanEdit = quizzes.Zip(quizzesDtos, (quiz, dto) =>
            {
                var canEdit = dto.CreatorId == currentUser.Id ||
                              quiz.QuizPermissions.FirstOrDefault(qp => qp.QuizId == dto.Id && qp.UserId == currentUser.Id)?.CanEdit == true;

                return dto with { CanEdit = canEdit };
            });

            quizzesDtos = dtosWithUpdatedCanEdit;
        }

        var paginationInfo = new PaginationInfo(totalCount, request.PageSize, request.PageNumber);

        return new PagedResponse<QuizDto>(quizzesDtos, paginationInfo);
    }
}
