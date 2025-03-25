namespace QuizAI.Domain.Interfaces;

public interface IEmailConsumerService
{
    Task InitializeAsync();
    Task StartConsumingAsync();
    Task StopConsumingAsync();
}
