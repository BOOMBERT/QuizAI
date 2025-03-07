using Microsoft.AspNetCore.Identity.UI.Services;
using Newtonsoft.Json;
using QuizAI.Application.Common;
using QuizAI.Application.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace QuizAI.Application.Services;

public class EmailConsumerService : IEmailConsumerService
{
    private readonly IEmailSender _emailSender;
    private readonly string _queueName;
    private IConnection _connection;
    private IChannel _channel;

    public EmailConsumerService(IEmailSender emailSender, string queueName)
    {
        _emailSender = emailSender;
        _queueName = queueName;
    }

    public async Task InitializeAsync()
    {
        var factory = new ConnectionFactory() { HostName = "localhost" };
        _connection = await factory.CreateConnectionAsync();
        _channel = await _connection.CreateChannelAsync();

        await _channel.QueueDeclareAsync(
            queue: _queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null
        );
    }

    public async Task StartConsumingAsync()
    {
        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var emailMessage = JsonConvert.DeserializeObject<EmailMessage>(message);

            await _emailSender.SendEmailAsync(emailMessage!.ToEmail, emailMessage.Subject, emailMessage.HtmlMessage);
        };

        await _channel.BasicConsumeAsync(
            queue: _queueName,
            autoAck: true,
            consumer: consumer);
    }

    public async Task StopConsumingAsync()
    {
        if (_channel != null && _channel.IsOpen)
            await _channel.CloseAsync();

        if (_connection != null && _connection.IsOpen)
            await _connection.CloseAsync();
    }
}
