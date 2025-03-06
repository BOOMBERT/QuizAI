using Microsoft.AspNetCore.Identity;

namespace QuizAI.Domain.Entities;

public class User : IdentityUser
{
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiresAtUtc { get; set; }

    public ICollection<QuizPermission> QuizPermissions { get; set; } = new List<QuizPermission>();
    public ICollection<QuizAttempt> QuizAttempts { get; set; } = new List<QuizAttempt>();
    public ICollection<Quiz> CreatedQuizzes { get; set; } = new List<Quiz>();

    public static User Create(string email)
    {
        return new User
        {
            Email = email,
            UserName = email
        };
    }
}
