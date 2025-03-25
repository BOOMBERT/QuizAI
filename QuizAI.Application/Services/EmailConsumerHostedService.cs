using Microsoft.Extensions.Hosting;
using QuizAI.Domain.Interfaces;

namespace QuizAI.Application.Services;

public class EmailConsumerHostedService : IHostedService
{
    private readonly IEmailConsumerService _emailConsumerService;

    public EmailConsumerHostedService(IEmailConsumerService emailConsumerService)
    {
        _emailConsumerService = emailConsumerService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await _emailConsumerService.InitializeAsync();
        await _emailConsumerService.StartConsumingAsync();
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _emailConsumerService.StopConsumingAsync();
    }
}
