using AutoMapper;
using MediatR;
using QuizAI.Application.Common;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Quizzes.Dtos;
using QuizAI.Domain.Entities;
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

        if (request.FilterByCreator)
        {
            quizzesDtos = quizzesDtos.Select(dto => dto with { CanEdit = true });
        }
        else
        {
            quizzesDtos = quizzesDtos.Zip(quizzes, (dto, quiz) => dto with
            {
                CanEdit = dto.CreatorId == currentUser.Id || quiz.QuizPermissions.Any(qp => qp.UserId == currentUser.Id && qp.CanEdit)
            });
        }

        if (request.FilterByUnfinishedAttempts)
        {
            quizzesDtos = quizzesDtos.Select(dto => dto with { HasUnfinishedAttempt = true });
        }
        else
        {
            quizzesDtos = quizzesDtos.Zip(quizzes, (dto, quiz) => dto with
            {
                HasUnfinishedAttempt = quiz.QuizAttempts.Any(qa => qa.UserId == currentUser.Id && qa.FinishedAt == null)
            });
        }

        quizzesDtos = quizzesDtos.Zip(quizzes, (dto, quiz) =>
        {
            string? publicImageUrl = null;
            if (!dto.IsPrivate && dto.HasImage)
            {
                publicImageUrl = $"/api/uploads/{quiz.ImageId}{quiz.Image!.FileExtension}";
            }

            return dto with { PublicImageUrl = publicImageUrl };
        });


        var paginationInfo = new PaginationInfo(totalCount, request.PageSize, request.PageNumber);

        return new PagedResponse<QuizDto>(quizzesDtos, paginationInfo);
    }
}
