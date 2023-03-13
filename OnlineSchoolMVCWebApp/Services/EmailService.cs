using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace OnlineSchoolMVCWebApp.Services
{
    public class EmailService
    {
        private readonly MailSettings settings;
        public EmailService(IOptions<MailSettings> settings)
        {

            this.settings = settings.Value;

        }
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();

            emailMessage.From.Add(new MailboxAddress("Адміністрація сайту", settings.From));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using (var client = new SmtpClient())
            {
                if(settings.UseSSL)
                {
                    await client.ConnectAsync(settings.Host, settings.Port, SecureSocketOptions.SslOnConnect);
                }
                else if (settings.UseStartTls)
                {
                    await client.ConnectAsync(settings.Host, settings.Port, SecureSocketOptions.StartTls);
                }
                await client.AuthenticateAsync(settings.UserName, settings.Password);
                await client.SendAsync(emailMessage);

                await client.DisconnectAsync(true);
            }
        }
    }
}
