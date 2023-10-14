using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using System.Net.Mail;
using TestApp.Database.Models;
using TestApp.Services.Interfaces;

namespace WebShop.Application.Services
{
    public class EmailService: IEmailService
    {
        private readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration) 
        {
            _configuration = configuration;
        }

        public void SendEmail(Email email)
        {
            var emailMessage = new MimeMessage();
            var from = _configuration.GetSection("EmailSettings:From").Value;
            emailMessage.From.Add(new MailboxAddress("Test Name", from));
            emailMessage.To.Add(new MailboxAddress(email.To, email.To));
            emailMessage.Subject = email.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = string.Format(email.Content)
            };

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    client.Connect(_configuration.GetSection("EmailSettings:SmtpServer").Value, Int32.Parse(_configuration.GetSection("EmailSettings:Port").Value), SecureSocketOptions.StartTls);
                    client.Authenticate(_configuration.GetSection("EmailSettings:Username").Value, _configuration.GetSection("EmailSettings:Password").Value);
                    client.Send(emailMessage);

                }
                catch (Exception e)
                {
                    throw e;
                }finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
            }
        }

        public string CreateEmailBody(string email, string emailToken)
        {
            return $@"<html>
                        <head>
                        </head>
                        <body>
                            <div>
                                <p>To reset your password please clic the link below</p>
                                <a href=""http://localhost:4200/passwordreset?email={email}&code={emailToken}"" style=""background: blue; color: white; padding: 8px 15px; border-radius: 8px;"">Reset password</a>
                            </div>
                        </body>
                    </html>";
        }
    }
}
