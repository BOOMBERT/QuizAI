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

    public GetAllQuizzesQueryHandler(IMapper mapper, IUserContext userContext, IQuizzesRepository quizzesRepository)
    {
        _mapper = mapper;
        _userContext = userContext;
        _quizzesRepository = quizzesRepository;
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
            request.FilterBySharedQuizzes,
            request.FilterByUnfinishedAttempts
        );

        var quizzesDtos = _mapper.Map<IEnumerable<QuizDto>>(quizzes);

        quizzesDtos = quizzesDtos.Zip(quizzes, (dto, quiz) =>
        {
            bool canEdit = dto.IsDeprecated 
                ? false
                : (request.FilterByCreator
                    ? true
                    : dto.CreatorId == currentUser.Id || quiz.QuizPermissions.Any(qp => qp.UserId == currentUser.Id && qp.CanEdit));

            bool hasUnfinishedAttempt = request.FilterByUnfinishedAttempts
                ? true
                : quiz.QuizAttempts.Any(qa => qa.UserId == currentUser.Id && qa.FinishedAt == null);

            string? publicImageUrl = (!dto.IsPrivate && dto.HasImage)
                ? $"/api/uploads/{quiz.ImageId}{quiz.Image!.FileExtension}"
                : null;

            return dto with
            {
                CanEdit = canEdit,
                HasUnfinishedAttempt = hasUnfinishedAttempt,
                PublicImageUrl = publicImageUrl
            };
        });

        var paginationInfo = new PaginationInfo(totalCount, request.PageSize, request.PageNumber);

        return new PagedResponse<QuizDto>(quizzesDtos, paginationInfo);
    }
}
