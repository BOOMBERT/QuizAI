namespace QuizAI.Application.Interfaces;

public interface IEmailConsumerService
{
    Task InitializeAsync();
    Task StartConsumingAsync();
    Task StopConsumingAsync();
}
