namespace Identity.Core.Application.DTOs.Account
{
    public class LoginAccountDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
