using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MailKit.Net.Smtp; //allows sending email using SMTP 
using MimeKit; //Helps create email messages with a subject, body, and attachments 
using Microsoft.Extensions.Configuration; //used to read setting from appsettings.json
using System.Threading.Tasks;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;  //Allows async programming to send emails without blocking the main thread 

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
                var smtpPort = int.Parse(_config["EmailSettings:SMTP_Port"]!);
                var smtpUsername = _config["EmailSettings:SMTP_Username"];
                var smtpPassword = _config["EmailSettings:SMTP_Password"];

                var email = new MimeMessage();
                email.From.Add(new MailboxAddress("Chess App", smtpUsername));
                email.To.Add(new MailboxAddress("", toEmail)); //Recipient email
                email.Subject = subject;
                email.Body = new TextPart("plain")
                {
                    Text = message
                };

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(smtpServer, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(smtpUsername, smtpPassword);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
                Debug.WriteLine("Email sent successfully");
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error sending email: {ex.Message}");
                return false; 
            }
        }

    }
}
