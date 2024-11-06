namespace QuizAI.Domain.Entities;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }

    public ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
}
