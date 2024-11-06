namespace QuizAI.Domain.Entities;

public class Image
{
    public int Id { get; set; }
    public string FilePath { get; set; }
    public string Name { get; set; }
    public string FileExtension { get; set; }
    public string Hash { get; set; }

    public ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
    public ICollection<Question> Questions { get; set; } = new List<Question>();
}
