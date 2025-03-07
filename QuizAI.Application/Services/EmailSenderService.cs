using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace QuizAI.Application.Services;

public class EmailSenderService : IEmailSender
{
    private readonly string _smtpServer;
    private readonly string _fromEmail;
    private readonly string _password;
    private readonly int _port;

    public EmailSenderService(string smptServer, string fromEmail, string password, int port)
    {
        _smtpServer = smptServer;
        _fromEmail = fromEmail;
        _password = password;
        _port = port;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var smtpClient = new SmtpClient(_smtpServer, _port)
        {
            UseDefaultCredentials = false,
            EnableSsl = true,
            Credentials = new NetworkCredential(_fromEmail, _password)
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_fromEmail),
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true
        };
        mailMessage.To.Add(new MailAddress(email));

        await smtpClient.SendMailAsync(mailMessage);
    }
}
