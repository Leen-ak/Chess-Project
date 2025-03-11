using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using DAL;
using MailKit.Security;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MailKit.Net.Smtp;
using Org.BouncyCastle.Crypto.Macs;

namespace BusinessLogic
{
    public class PasswordRecoveryService
    {
        private readonly PasswordDAO _passwordDAO;

        public PasswordRecoveryService()
        {
            _passwordDAO = new PasswordDAO();
        }


        public async Task<int?> GetIdByEmail(string email)
        {
            var user = await _passwordDAO.GetIdByEmail(email);
            Debug.WriteLine($"User id is {user}"); 
            return user == null ? throw new InvalidOperationException("Id not found") : user.Value;
        }

        public async Task<string?> GetEmail(int? id, string email)
        {
            var userEmail = await _passwordDAO.GetEmail(id, email);
            if (userEmail == null) 
                throw new Exception("Email not found for the given User ID.");
            if (userEmail != email)
                Debug.WriteLine("The user email does not match"); 
            return userEmail;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string message)
        {
            try
            {
                var smtpServer = "smtp.gmail.com";
                var smtpPort = 587;
                var smtpUsername = "leen.lsa.39@gmail.com";
                var smtpPassword = "pqsfwctzvcijmpqy";

                var email = new MimeMessage();
                email.From.Add(new MailboxAddress("The Chess Gambit", smtpUsername));
                email.To.Add(new MailboxAddress("", toEmail));
                email.Subject = subject;
                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = message 
                };
                email.Body = bodyBuilder.ToMessageBody();

                using var smtp = new SmtpClient();
                await smtp.ConnectAsync(smtpServer, smtpPort, SecureSocketOptions.StartTls);
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
 