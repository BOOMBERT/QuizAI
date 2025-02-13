using MediatR;
using QuizAI.Application.Interfaces;
using QuizAI.Application.QuizPermissions.Dtos;
using QuizAI.Application.Users;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Exceptions;
using QuizAI.Domain.Repositories;

namespace QuizAI.Application.QuizPermissions.Queries.GetAllQuizUsersPermissions;

public class GetAllQuizUsersPermissionsQueryHandler : IRequestHandler<GetAllQuizUsersPermissionsQuery, IEnumerable<QuizUsersPermissionsDto>>
{
    private readonly IUserContext _userContext;
    private readonly IUserRepository _userRepository;
    private readonly IQuizzesRepository _quizzesRepository;
    private readonly IQuizPermissionsRepository _quizPermissionsRepository;

    public GetAllQuizUsersPermissionsQueryHandler(
        IUserContext userContext, IUserRepository userRepository, IQuizzesRepository quizzesRepository, IQuizPermissionsRepository quizPermissionsRepository)
    {
        _userContext = userContext;
        _userRepository = userRepository;
        _quizzesRepository = quizzesRepository;
        _quizPermissionsRepository = quizPermissionsRepository;
    }

    public async Task<IEnumerable<QuizUsersPermissionsDto>> Handle(GetAllQuizUsersPermissionsQuery request, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.GetCurrentUser();

        var (creatorId, isDeprecated) = await _quizzesRepository.GetCreatorIdAndIsDeprecatedAsync(request.QuizId)
            ?? throw new NotFoundException($"Quiz with ID {request.QuizId} was not found");

        if (isDeprecated)
            throw new NotFoundException($"Quiz with ID {request.QuizId} was not found");

        if (creatorId != currentUser.Id)
            throw new ConflictException($"You cannot view the permissions for quiz with ID {request.QuizId} because you are not its creator");

        var quizPermissions = await _quizPermissionsRepository.GetAllAsync(request.QuizId);
        var quizUsersPermissionsDtos = await Task.WhenAll(
            quizPermissions.Select(async qp =>
            {
                var userEmail = await _userRepository.GetEmailAsync(qp.UserId);

                return new QuizUsersPermissionsDto(
                    qp.Id,
                    userEmail!,
                    qp.CanEdit,
                    qp.CanPlay
                );
            })
        );

        return quizUsersPermissionsDtos;
    }
}
