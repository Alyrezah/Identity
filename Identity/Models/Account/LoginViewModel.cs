using Microsoft.AspNetCore.Authentication;

namespace Identity.Models.Account
{
    public class LoginViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public bool RememberMe { get; set; }

        public string ReturnUrl { get; set; }
        public List<AuthenticationScheme> ExternalLogin { get; set; }
    }
}
