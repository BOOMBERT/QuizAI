using QuizAI.Application.Users;

namespace QuizAI.Application.Interfaces;

public interface IUserContext
{
    CurrentUser GetCurrentUser();
}
