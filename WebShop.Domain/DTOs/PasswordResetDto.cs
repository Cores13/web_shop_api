namespace WebShop.Domain.DTOs
{
    public class PasswordResetDto
    {
        public string Email { get; set; }
        public string EmailToken { get; set; }
        public string NewPassword { get; set; }
        public string PasswordConfirm { get; set; }
    }
}
