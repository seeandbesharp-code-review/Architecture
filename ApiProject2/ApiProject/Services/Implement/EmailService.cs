using MailKit.Security;
using MimeKit;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using ApiProject.Services.Interface;

namespace ApiProject.Services.Implement
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailService> _logger; 

        public EmailService(IConfiguration config, ILogger<EmailService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendWinnerEmail(string email, string winnerName, string giftName)
        {
            try
            {
                _logger.LogInformation("Attempting to send winner email to {Email} for gift {GiftName}", email, giftName);

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("מערכת מכירה סינית", _config["Email:EmailAddress"]));
                message.To.Add(MailboxAddress.Parse(email));
                message.Subject = "🎉 זכית בהגרלה!";

                message.Body = new TextPart("plain")
                {
                    Text = $"שלום {winnerName},\n\nמזל טוב! זכית בהגרלה עבור המתנה: {giftName}\n\nתודה על השתתפותך במכירה הסינית 🌸"
                };

                using (var client = new SmtpClient())
                {
                    // חיבור לשרת המייל
                    await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

                    // אימות מול השרת
                    await client.AuthenticateAsync(_config["Email:EmailAddress"], _config["Email:AppPassword"]);

                    // שליחה
                    await client.SendAsync(message);

                    // ניתוק
                    await client.DisconnectAsync(true);
                }

                _logger.LogInformation("Email successfully sent to {Email}", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send winner email to {Email}", email);
                throw;
            }
        }
    }
}