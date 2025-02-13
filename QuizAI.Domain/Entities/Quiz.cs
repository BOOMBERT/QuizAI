namespace QuizAI.Domain.Entities;

public class Quiz
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTime CreationDate { get; set; }
    public bool IsPrivate { get; set; }
    public bool IsDeprecated { get; set; }
    public Guid? LatestVersionId { get; set; }
    public int QuestionCount { get; set; }
    
    public string CreatorId { get; set; }
    public User Creator { get; set; }
    public Guid? ImageId { get; set; }    
    public Image? Image { get; set; }
    public QuizPermission QuizPermission { get; set; }
    public ICollection<Category> Categories { get; set; } = new List<Category>();
    public ICollection<Question> Questions { get; set; } = new List<Question>();
    public ICollection<QuizAttempt> QuizAttempts { get; set; } = new List<QuizAttempt>();
}
