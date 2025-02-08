using Microsoft.AspNetCore.Identity;

namespace QuizAI.Domain.Entities;

public class User : IdentityUser
{
    public ICollection<QuizAttempt> QuizAttempts { get; set; } = new List<QuizAttempt>();
    public ICollection<Quiz> CreatedQuizzes { get; set; } = new List<Quiz>();
}
