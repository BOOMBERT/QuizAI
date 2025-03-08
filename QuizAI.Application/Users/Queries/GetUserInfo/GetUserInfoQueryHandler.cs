using MediatR;
using QuizAI.Application.Interfaces;
using QuizAI.Application.Users.Dtos;

namespace QuizAI.Application.Users.Queries.GetUserInfo;

public class GetUserInfoQueryHandler : IRequestHandler<GetUserInfoQuery, UserDto>
{
    private readonly IUserContext _userContext;

    public GetUserInfoQueryHandler(IUserContext userContext)
    {
        _userContext = userContext;
    }

    public async Task<UserDto> Handle(GetUserInfoQuery request, CancellationToken cancellationToken)
    {
        var currentUser = _userContext.GetCurrentUser();

        return await Task.FromResult(new UserDto(currentUser.Email));
    }
}
