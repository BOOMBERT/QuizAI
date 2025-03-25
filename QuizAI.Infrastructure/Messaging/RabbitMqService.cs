using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using QuizAI.Application.Common;
using QuizAI.Domain.Entities;
using QuizAI.Domain.Interfaces;
using RabbitMQ.Client;
using System.Text;

namespace QuizAI.Infrastructure.Messaging;

public class RabbitMqService : IRabbitMqService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly string _queueName;
    private readonly string _hostName;
    private readonly int _port;

    public RabbitMqService(IHttpContextAccessor httpContextAccessor, string queueName, string hostName, int port)
    {
        _httpContextAccessor = httpContextAccessor;
        _queueName = queueName;
        _hostName = hostName;
        _port = port;
    }

    public async Task SendEmailToQueueAsync(string toEmail, string subject, string htmlMessage)
    {
        var factory = new ConnectionFactory() { HostName = _hostName, Port = _port };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: _queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
            );

        var emailMessage = new EmailMessage
        {
            ToEmail = toEmail,
            Subject = subject,
            HtmlMessage = htmlMessage
        };

        string emailMessageJson = JsonConvert.SerializeObject(emailMessage);
        var body = Encoding.UTF8.GetBytes(emailMessageJson);

        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: _queueName,
            body: body);
    }

    public async Task SendRegistrationConfirmationEmailToQueueAsync(User user, string token)
    {
        var httpContextRequest = _httpContextAccessor.HttpContext!.Request;
        var confirmationLink = $"{httpContextRequest.Scheme}://{httpContextRequest.Host}/api/users/confirm-email?Id={user.Id}&Token={token}";

        await SendEmailToQueueAsync(
            toEmail: user.Email!,
            subject: "Complete Your Registration – Confirm Your Email",
            htmlMessage: $"""
                <h1 style="color: #4b3bc4;">Hello {user.UserName!.Split('@')[0]},</h1><br>
                <h2 style="color: #4b3bc4;">Please confirm your email address by clicking the link below:</h2>
                <h2 style="color: #4b3bc4;"><a href='{confirmationLink}'>Confirm Email</a></h2>
                """);
    }
}
