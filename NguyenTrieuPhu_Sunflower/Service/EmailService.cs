
using System.Net.Mail;
using System.Net;
using MimeKit;
using MailKit.Net.Smtp;
namespace NguyenTrieuPhu_Sunflower.Service
{
    public class EmailService : IEmailService
    {
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress("memory_map", "your-email@gmail.com"));
            email.To.Add(new MailboxAddress("", to));
            email.Subject = subject;

            email.Body = new TextPart("plain")
            {
                Text = body
            };

            using var smtpClient = new MailKit.Net.Smtp.SmtpClient();
            await smtpClient.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
            await smtpClient.AuthenticateAsync("phunguyentrieu3@gmail.com", "wrzv bobk qbyf pkaq"); // Mật khẩu ứng dụng
            await smtpClient.SendAsync(email);
            await smtpClient.DisconnectAsync(true);
        }
    }
}
