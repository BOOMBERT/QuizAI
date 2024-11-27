namespace QuizAI.Domain.Entities;

public class Image
{
    public Guid Id { get; set; }
    public string FileExtension { get; set; }
    public byte[] Hash { get; set; }

    public ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
    public ICollection<Question> Questions { get; set; } = new List<Question>();
}
