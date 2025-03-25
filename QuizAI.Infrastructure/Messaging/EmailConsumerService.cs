using QuizAI.Domain.Exceptions;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using QuizAI.Domain.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;
using QuizAI.Application.Common;
using Newtonsoft.Json;
using System.Text;

namespace QuizAI.Infrastructure.Messaging;

public class EmailConsumerService : IEmailConsumerService
{
    private readonly IEmailSender _emailSender;
    private readonly string _queueName;
    private readonly string _hostName;
    private readonly int _port;
    private IConnection? _connection;
    private IChannel? _channel;

    public EmailConsumerService(IEmailSender emailSender, string queueName, string hostName, int port)
    {
        _emailSender = emailSender;
        _queueName = queueName;
        _hostName = hostName;
        _port = port;
    }

    public async Task InitializeAsync()
    {
        var factory = new ConnectionFactory() { HostName = _hostName, Port = _port };
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
        if (_connection == null || _channel == null)
            throw new ConflictException("Cannot start consuming: the connection or channel is not initialized");

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
