namespace Identity.Core.Application.DTOs.Account
{
    public class RegisterAccountDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string RePassword { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
