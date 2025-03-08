using QuizAI.Domain.Entities;

namespace QuizAI.Application.Interfaces;

public interface IRabbitMqService
{
    Task SendEmailToQueueAsync(string toEmail, string subject, string htmlMessage);
    Task SendRegistrationConfirmationEmailToQueueAsync(User user, string token);
}
