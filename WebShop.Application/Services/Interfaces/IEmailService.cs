using TestApp.Database.Models;

namespace TestApp.Services.Interfaces
{
    public interface IEmailService
    {
        void SendEmail(Email email);
        string CreateEmailBody(string email, string emailToken);
    }
}
