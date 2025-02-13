namespace QuizAI.Domain.Repositories;

public interface IUserRepository
{
    Task<string?> GetIdAsync(string email);
    Task<string?> GetEmailAsync(string id);
}
