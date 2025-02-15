using Microsoft.AspNetCore.Identity;

namespace QuizAI.Domain.Entities;

public class User : IdentityUser
{
    public ICollection<QuizPermission> QuizPermissions { get; set; } = new List<QuizPermission>();
    public ICollection<QuizAttempt> QuizAttempts { get; set; } = new List<QuizAttempt>();
    public ICollection<Quiz> CreatedQuizzes { get; set; } = new List<Quiz>();
}
