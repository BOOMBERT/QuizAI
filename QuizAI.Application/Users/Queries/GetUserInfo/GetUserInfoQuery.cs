using MediatR;
using QuizAI.Application.Users.Dtos;

namespace QuizAI.Application.Users.Queries.GetUserInfo;

public class GetUserInfoQuery : IRequest<UserDto>
{
}
