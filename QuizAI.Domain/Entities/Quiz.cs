namespace QuizAI.Domain.Entities;

public class Quiz
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public DateTime CreationDate { get; set; }

    public int? ImageId { get; set; }    
    public Image? Image { get; set; }

    public ICollection<Category> Categories { get; set; } = new List<Category>();

    public ICollection<Question> Questions { get; set; } = new List<Question>();
}
