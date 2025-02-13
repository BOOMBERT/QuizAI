﻿using AutoMapper;
using MediatR;
using QuizAI.Application.Common;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Quizzes.Dtos;
using QuizAI.Application.Users;
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
        var (quizzes, totalCount) = await _quizzesRepository.GetAllMatchingAsync(
            request.SearchPhrase,
            request.PageSize,
            request.PageNumber,
            request.SortBy,
            request.SortDirection,
            request.FilterByUser ? _userContext.GetCurrentUser().Id : null,
            request.FilterCategories
        );

        var quizzesDtos = _mapper.Map<IEnumerable<QuizDto>>(quizzes);
        var paginationInfo = new PaginationInfo(totalCount, request.PageSize, request.PageNumber);

        return new PagedResponse<QuizDto>(quizzesDtos, paginationInfo);
    }
}
