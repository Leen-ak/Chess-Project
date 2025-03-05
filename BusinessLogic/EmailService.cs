using System;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;

namespace BusinessLogic
{
    public class EmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string message)
        {
            try
            {
                var smtpServer = _config["EmailSettings:SMTP_Server"];
                var smtpPort = int.Parse(_config["EmailSettings:SMTP_Port"]);
                var smtpUsername = _config["EmailSettings:SMTP_Username"];
                var smtpPassword = _config["EmailSettings:SMTP_Password"];
                var useSSL = bool.Parse(_config["EmailSettings:UseSSL"]);

                var email = new MimeMessage();
                email.From.Add(new MailboxAddress("The Chess Gambit", smtpUsername));
                email.To.Add(new MailboxAddress("", toEmail));
                email.Subject = subject;
                email.Body = new TextPart("plain") { Text = message };

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(smtpServer, smtpPort, useSSL ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(smtpUsername, smtpPassword);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                Console.WriteLine("Email sent successfully!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
        }
    }
}
