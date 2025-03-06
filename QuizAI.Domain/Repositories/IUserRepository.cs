using QuizAI.Domain.Entities;

namespace QuizAI.Domain.Repositories;

public interface IUserRepository
{
    Task<string?> GetIdAsync(string email);
    Task<string?> GetEmailAsync(string id);
    Task<bool> ExistsByEmailAsync(string email);
    Task<User?> GetUserByRefreshTokenAsync(string refreshToken);
}
