
using Microsoft.AspNetCore.Identity.UI.Services;

using System.Net;
using System.Net.Mail;

namespace AccReporting.Server;

public class EmailService : IEmailSender
{
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        const string fromMail = "farooqtahir1234@gmail.com";
        const string fromPassword = "fyodjikehsunvmdq";

        MailMessage message = new();
        message.From = new MailAddress(fromMail);
        message.Subject = subject;
        message.To.Add(new MailAddress(email));
        message.Body = htmlMessage;
        message.IsBodyHtml = true;

        var smtpClient = new SmtpClient("smtp.gmail.com")
        {
            Port = 587,
            Credentials = new NetworkCredential(fromMail, fromPassword),
            EnableSsl = true,
        };
        await smtpClient.SendMailAsync(message);
    }
}

