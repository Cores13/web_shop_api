using WebShop.Domain.Entities;

namespace WebShop.Application.Interfaces
{
    public interface IEmailService
    {
        void SendEmail(Email email);
        string CreateEmailBody(string email, string emailToken);
    }
}
