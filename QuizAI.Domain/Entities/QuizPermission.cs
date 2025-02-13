namespace QuizAI.Domain.Entities;

public class QuizPermission
{
    public Guid Id { get; set; }
    public Guid QuizId { get; set; }
    public Quiz Quiz { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }
    public bool CanEdit { get; set; }
    public bool CanPlay { get; set; }
}
